using System;
using System.Collections.Generic;


namespace UNM.Parser
{
    /// <summary>
    /// An interface for providing Namelists to the <see cref="NameParser"/>
    /// </summary>
    public interface INamelistSource
    {
        /// <summary>
        /// Initialize the INamelistSource. Used to pre-load namelists from whatever source.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Retrieve a Namelist from this INamelistSource
        /// </summary>
        /// <param name="name">The name of the list to retrieve.</param>
        /// <returns>The Namelist matching <paramref name="name"/></returns>
        /// <throws>ArgumentException if the INamelistSource does not contain a list matching <paramref name="name"/></throws>
        Namelist GetNamelist(string name);
    }
}
