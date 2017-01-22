# Universal Name Markup #

This README would normally document whatever steps are necessary to get your application up and running.


# Fragments, NameLists, Context and Variables #

# Input Pattern Syntax #

The input pattern passed into the NameParser should be a text string containing angle bracket tags. For example: "this is an <example_synonym> input pattern" The NameParser will operate on the tags, performing replacements and conditional branching.

<namelist_name> - A fragment tag, NameParser will replace the tag with a fragment chosen from the namelist named within the tag itself.

<#varaible_name> - A varaible substitution tag.

<^namelist_name> - A sub pattern tag.

<%50> - A chance branch tag.

<@context_name> - A context branch tag.

<$variable_name> - A variable presence branch tag.

# NnameList File Syntax # 

# Capitalization Scheme #


--------------------------------------------

Code licensed under the MIT License. See LICENSE.txt for details.