﻿using Arvant.Entity.SelectQuery.Models;
using Arvant.Entity.Structure;

namespace Arvant.Entity.SelectQuery;

public class JoinsStorage(AliasStorage aliasStorage, string structureAlias, EntityStructure entityStructure)
{
    private readonly List<JoinInfo> _joins = new();
    
    public IEnumerable<JoinInfo> Joins => _joins;

    public readonly string StructureAlias = structureAlias;

    public readonly EntityStructure EntityStructure = entityStructure;
    
    public string BuildJoinAlias(string path, string fullPath, EntityStructure primaryStructure, 
        EntityStructure parentStructure) {
        var alias = aliasStorage.GetTableAlias(path);
        var joinInfo = new JoinInfo(fullPath, alias, primaryStructure, parentStructure);
        _joins.Add(joinInfo);
        return alias;
    }
    
    public string BuildJoinAlias(SelectExpression selectExpression, EntityStructure primaryStructure, 
        EntityStructure parentStructure) {
        var alias = aliasStorage.GetTableAlias(selectExpression.Path);
        var fullPath = selectExpression.GetFullPath();
        var joinInfo = new JoinInfo(fullPath, alias, primaryStructure, parentStructure);
        _joins.Add(joinInfo);
        return alias;
    }
    
    public JoinMatchResult FindJoinPath(string inputPath) {
        var match = _joins.Select(j => j.JoinPath).Where(inputPath.StartsWith).MaxBy(s => s.Length);
        if (match == null) {
            return new JoinMatchResult(null, inputPath);
        }
        var leftoverPath = inputPath[match.Length..].TrimStart('.');
        var matchedJoin = _joins.First(j => j.JoinPath == match);
        return new JoinMatchResult(matchedJoin, leftoverPath);
    }

    public void Clear() {
        _joins.Clear();
    }
}

public record JoinMatchResult(JoinInfo? Join, string LeftoverPath);
