﻿using System;
using Mysqlx.Expr;
using Mysqlx.Datatypes;
using Mysqlx.Crud;
using Google.ProtocolBuffers;

namespace MySql.Protocol.X
{
  internal class ExprUtil
  {
    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar NULL type.
     */
    public static Expr BuildLiteralNullScalar()
    {
      return BuildLiteralExpr(NullScalar());
    }

    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar DOUBLE type (wrapped in Any).
     */
    public static Expr BuildLiteralScalar(double d)
    {
      return BuildLiteralExpr(ScalarOf(d));
    }

    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar SINT (signed int) type (wrapped in Any).
     */
    public static Expr BuildLiteralScalar(long l)
    {
      return BuildLiteralExpr(ScalarOf(l));
    }

    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar STRING type (wrapped in Any).
     */
    public static Expr BuildLiteralScalar(String str)
    {
      return BuildLiteralExpr(ScalarOf(str));
    }

    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar OCTETS type (wrapped in Any).
     */
    public static Expr BuildLiteralScalar(byte[] bytes)
    {
      return BuildLiteralExpr(ScalarOf(bytes));
    }

    /**
     * Proto-buf helper to build a LITERAL Expr with a Scalar BOOL type (wrapped in Any).
     */
    public static Expr BuildLiteralScalar(Boolean b)
    {
      return BuildLiteralExpr(ScalarOf(b));
    }

    /**
     * Wrap an Any value in a LITERAL expression.
     */
    public static Expr BuildLiteralExpr(Scalar scalar)
    {
      return Expr.CreateBuilder().SetType(Expr.Types.Type.LITERAL).SetLiteral(scalar).Build();
    }

    public static Scalar NullScalar()
    {
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_NULL).Build();
    }

    public static Scalar ScalarOf(double d)
    {
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_DOUBLE).SetVDouble(d).Build();
    }

    public static Scalar ScalarOf(long l)
    {
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_SINT).SetVSignedInt(l).Build();
    }

    public static Scalar ScalarOf(String str)
    {
      Scalar.Types.String strValue = Scalar.Types.String.CreateBuilder().SetValue(ByteString.CopyFromUtf8(str)).Build();
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_STRING).SetVString(strValue).Build();
    }

    public static Scalar ScalarOf(byte[] bytes)
    {
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_OCTETS).SetVOpaque(ByteString.CopyFrom(bytes)).Build();
    }

    public static Scalar ScalarOf(Boolean b)
    {
      return Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_BOOL).SetVBool(b).Build();
    }

    /**
     * Build an Any with a string value.
     */
    public static Any BuildAny(String str)
    {
      // same as Expr
      Scalar.Types.String sstr = Scalar.Types.String.CreateBuilder().SetValue(ByteString.CopyFromUtf8(str)).Build();
      Scalar s = Scalar.CreateBuilder().SetType(Scalar.Types.Type.V_STRING).SetVString(sstr).Build();
      Any a = Any.CreateBuilder().SetType(Any.Types.Type.SCALAR).SetScalar(s).Build();
      return a;
    }

    public static Any BuildAny(Boolean b)
    {
      return Any.CreateBuilder().SetType(Any.Types.Type.SCALAR).SetScalar(ScalarOf(b)).Build();
    }

    public static Collection BuildCollection(String schemaName, String collectionName)
    {
      return Collection.CreateBuilder().SetSchema(schemaName).SetName(collectionName).Build();
    }

    public static Scalar ArgObjectToScalar(System.Object value)
    {
      return ArgObjectToExpr(value, false).Literal;
    }

    public static Expr ArgObjectToExpr(System.Object value, Boolean allowRelationalColumns)
    {
      if (value == null)
        return BuildLiteralNullScalar();

      if (value is bool)
        return BuildLiteralScalar((Boolean)value);
      else if (value is byte || value is short || value is int || value is long)
        return BuildLiteralScalar((long)value);
      else if (value is float || value is double)
        return BuildLiteralScalar((double)value);
      else if (value is string)
        return BuildLiteralScalar((string)value);
      throw new NotSupportedException("Value of type " + value.GetType() + " is not currently supported.");
    //} else if (value.getClass() == Expression.class) {
      //      return new ExprParser(((Expression) value).getExpressionString(), allowRelationalColumns).parse();
        //} else if (value.getClass() == JsonDoc.class) {
          //  // TODO: check how xplugin handles this
        //} else if (value.getClass() == JsonArray.class) {
            // TODO: check how xplugin handles this
       // }
        //throw new NullPointerException("TODO: other types? BigDecimal, Date, Timestamp, Time");
    }

  }
}
