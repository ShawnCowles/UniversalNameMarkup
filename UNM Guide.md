# Fragments, NameLists, Context and Variables #

At its basic level, Universal Name Markup parses the input pattern for tags, and substitutes with values randomly chosen from a namelist. The namelist is a list of fragments, each of which may be chosen and added to the output. The selection behavior can be altered by passing in "context" with the pattern. "Context" is just a list of strings representing the context of the text being generated. "Goblin" for example, may be passed as context when generating the name of a goblin tribe. Framgments may have a logical expression that operates on context to determine if the fragment is a viable subsitution for the present context. Only fragments who have no logical expression, or who's expression passes have the potential to be selected. Context may also be used for branching within the input pattern itself.

Variables may also be passed along with the input pattern. They consist of a name and a value. They are not considered when selecting fragments, but can be used for conditional branching within the pattern, or subsitution with the value of the variable replacing the variable subsitution tag.

# Input Pattern Syntax #

The input pattern passed into the NameParser should be a text string containing angle bracket tags. For example: "this is an <example_synonym> input pattern" The NameParser will operate on the tags, performing replacements and conditional branching.

    <namelist_name> - A fragment tag, NameParser will replace the tag with a fragment chosen from the namelist named within the tag itself.

    <#varaible_name> - A variable substitution tag. NameParser will replace the tag with the value of the specified variable.

    <^namelist_name> - A sub pattern tag. NameParser will replace the tag with a fragment chosen from the namelist named within the tag itself, then process the fragment as if it was a new pattern itself.

    <%50> - A chance branch tag. NameParser will evaluate the contents as a chance out of 100 (value/100 % chance). It must be followed by a block enclosed in braces { }, and may contain a pipe "|" to split into an else block. Ex: <%50>{the if branch|the else branch} Only the chosen branch will be outputted, along with any processed tags.

    <@context_name> - A context branch tag. The branch will be taken if the specified context is present. Uses same block syntax as the chance branch tag.

    <$variable_name> - A variable presence branch tag. The branch will be taken if the specified variable is present. Uses same block syntax as the chance branch tag.

# Context Expression Syntax #

The context expression format is straightforward. It should consist of a series of context names, joined by boolean operators. If a context name is present it will evaluate to true, false otherwise. AND (%%) OR (||) and NOT(!) operators are supported.

    a_context || another_context && ! some_other_one

# NameList File Syntax # 

NameParser requires a namelist source to read namelists from during initialization. The default is the FileStreamNamelistSource that reads from CSV files provided via a stream. The expected CSV format is straightforward, one fragment to a row. The first column should be the name of the namelist the fragment belongs two, the second column should be the fragment itself, the third column  should either be empty, or a context expression for the fragment.

# Capitalization Scheme #

NameParser can apply a capitalization scheme to the final output, depending on the type of text generated this can be used to capitalize proper nouns properly, or even capitalize appropriately for a sentence.

The options are:

    BY_FRAGMENT - EachFragment Is Capitalized

    BY_WORDS - Each Word Is Capitalized

    FIRST_LETTER - Only the first letter is capitalized

    NONE - no capitalization applied

    BY_SENTENCE - Capitalized appropriately for a sentence.

# Best Practices # 
Instead of keeping input patterns in your code base wherever NameParser needs to be called, exploited the sub pattern tag. This way you can store the pattern for names along with the namelists and fragments that will be used.

For example, instead of:
    var ppp = new PatternProcessingParameters("The <adjective> <noun><verber> of <noun>")
            {
                CapitalizationScheme = CapitalizationScheme.BY_WORDS,
            };

Do this:
    var ppp = new PatternProcessingParameters("<^_pattern_goblin_name>")
            {
                CapitalizationScheme = CapitalizationScheme.BY_WORDS,
            };
and wherever your namelists are stored add a fragment for the namelist "_pattern_goblin_name" of "The <adjective> <noun><verber> of <noun>". This approach also allows you to easily vary patterns by simply adding more fragments to each pattern namelist.