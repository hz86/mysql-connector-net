﻿// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using MySql.Communication;
using MySql.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MySql.XDevAPI
{
  public class ResultSet
  {
    public List<ResultRow> Rows = new List<ResultRow>();
    public List<Column> Columns = new List<Column>();
    private Dictionary<string, int> nameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    private bool _complete = false;
    internal ProtocolBase _protocol;
    public int Position { get; private set; }
    public int PageSize { get; private set; }

    internal ResultSet(ProtocolBase p)
    {
      _protocol = p;
      PageSize = 20;
      Position = -1;
    }

    internal void LoadMetadata()
    {
      ///TODO:  move this to the ctor
      Columns = _protocol.LoadColumnMetadata();
      for (int i = 0; i < Columns.Count; i++)
        nameMap.Add(Columns[i].Name, i);
    }

    public object this[int index]
    {
      get { return GetValue(index);  }
    }

    public void FinishLoading()
    {
      int _pos = Position;
      while (Next()) ;
      Position = _pos == -1 ? 0 : _pos;
    }

    public bool Next()
    {
      Position++;
      if (Position == Rows.Count)
      {
        if (_complete) return false;
        if (!PageInRows())
        {
          _complete = true;
          return false;
        }
      }
      return true;
    }

    internal void Dump()
    {
      if (_complete) return;
      while (true)
      {
        List<byte[]> values = _protocol.ReadRow();
        if (values == null) break;
      }
      _complete = true;
    }

    private bool PageInRows()
    {
      for (int i = 0; i < PageSize; i++)
        if (!ReadRow()) break;
      return Position < Rows.Count;
    }

    private bool ReadRow()
    {
      ///TODO:  fix this
      List<byte[]> values = _protocol.ReadRow();
      if (values == null) return false;
      Debug.Assert(values.Count == Columns.Count, "Value count does not equal column count");
      ResultRow row = new ResultRow(this, Columns.Count);
      row.SetValues(values);
      Rows.Add(row);
      return true;
    }

    private object GetValue(int index)
    {
      if (Position == Rows.Count)
        throw new InvalidOperationException("No data at position");
      return Rows[Position][index];
    }

    public int IndexOf(string name)
    {
      if (!nameMap.ContainsKey(name))
        throw new MySqlException("Column not found '" + name + "'");
      return nameMap[name];
    }
  }
}
