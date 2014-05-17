using System;
using System.Collections.Generic;
using System.IO;

namespace UNM.Parser
{
	public class UnmData
	{
		private List<NameList> _lists = new List<NameList>();
		
		public UnmData()
		{
		}
		
		public NameList GetList(string name)
		{
			foreach(NameList list in _lists)
			{
				if(list.Name == name)
				{
					return list;
				}
			}
			
			var newList = new NameList(name);
			_lists.Add(newList);
			return newList;
		}
	}
}

