﻿using System;
using System.Collections.Generic;
using UNM.Parser.SimpleLexer;

namespace UNM.GCS.Data.ExpressionNodes
{
    internal class ValueNode : AbstractNode
    {
        internal ValueNode(Token token)
            :base(token)
        {
        }

        internal override bool Evaluate(Dictionary<string, string> variables)
        {
            throw new NotImplementedException();
        }
    }
}
