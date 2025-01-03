﻿using Arvant.Entity.SelectQuery.Enums;
using Arvant.Entity.SelectQuery.Models;
using Arvant.Entity.Structure;
using Arvant.Entity.Utils;
using Newtonsoft.Json.Linq;

namespace Arvant.Entity.SelectQuery;

public class RowReader(IDictionary<string, object> row, EntityStructure rootStructure) 
{
    public JObject ReadExpressions(List<SelectExpression> expressions) {
        var jsonObject = new JObject();
        foreach (var expression in expressions) {
            if (expression.Type == ExpressionType.Column) {
                var columnValue = ReadColumn(expression, rootStructure);
                jsonObject.Add(SelectQueryUtils.GetExpressionAlias(expression), columnValue);
            }
            if (expression.Type == ExpressionType.SubQuery || expression.Type == ExpressionType.Function) {
                var subQueryValue = ReadExpressionValue(expression);
                jsonObject.Add(expression.Alias!, subQueryValue);
            }
        }
        return jsonObject;
    }
    
    private JToken ReadColumn(SelectExpression columnExpression, EntityStructure entityStructure) {
        var entityColumn = entityStructure.Columns.FirstOrDefault(c => c.Name == columnExpression.Path);
        if (entityColumn == null) return JValue.CreateNull();
        if (!entityColumn.IsForeignKey) {
            return ReadExpressionValue(columnExpression);
        }
        if (entityColumn.ForeignKeyStructure?.ReferenceEntityStructure == null) return JValue.CreateNull();
        var foreignStructure = entityColumn.ForeignKeyStructure.ReferenceEntityStructure;
        var foreignObject = ReadForeignEntity(foreignStructure, columnExpression.Columns);
        return foreignObject;
    }
    
    private JToken ReadForeignEntity(EntityStructure entityStructure, List<SelectExpression> columns) {
        var pkExpression = columns.FirstOrDefault(
            c => c.Type == ExpressionType.Column && c.Path == entityStructure.PrimaryColumn.Name);
        if (pkExpression == null) {
            return JValue.CreateNull();
        }
        if (!TryReadPrimaryColumn(pkExpression, out var primaryColumnValue)) {
            return primaryColumnValue;
        }
        var jsonObject = new JObject {
            { SelectQueryUtils.GetExpressionAlias(pkExpression), primaryColumnValue }
        };
        var columnExpressions = columns.Where(c => c.Type == ExpressionType.Column && 
                                                   c.Path != entityStructure.PrimaryColumn.Name);
        foreach (var columnExpression in columnExpressions) {
            var columnValue = ReadColumn(columnExpression, entityStructure);
            jsonObject.Add(SelectQueryUtils.GetExpressionAlias(columnExpression), columnValue);
        }
        var subQueryExpressions = columns.Where(c => c.Type == ExpressionType.SubQuery);
        foreach (var subQueryExpression in subQueryExpressions) {
            var subQueryValue = ReadExpressionValue(subQueryExpression);
            jsonObject.Add(subQueryExpression.Alias!, subQueryValue);
        }
        return jsonObject;
    }

    private bool TryReadPrimaryColumn(SelectExpression pkExpression, out JToken primaryValue) {
        if (string.IsNullOrEmpty(pkExpression.SelectAlias)) {
            primaryValue = JValue.CreateNull();
            return false;
        }
        var primaryColumnValue = ReadRowKey(pkExpression.SelectAlias);
        if (primaryColumnValue == null) {
            primaryValue = JValue.CreateNull();
            return false;
        }
        primaryValue = JToken.FromObject(primaryColumnValue);
        return true;
    }
    
    private JToken ReadExpressionValue(SelectExpression columnExpression) {
        var columnValue = ReadRowKey(columnExpression.SelectAlias);
        return columnValue == null ? JValue.CreateNull() : JToken.FromObject(columnValue);
    }

    private object? ReadRowKey(string? key) {
        if(string.IsNullOrEmpty(key)) return null;
        return row.TryGetValue(key, out var rowValue) ? rowValue : null;
    }
}
