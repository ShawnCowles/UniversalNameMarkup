# The Generic Conversation System #

The GCS is built around the concept of a list of topics, each containing multiple possible responses.

The core of the Generic Conversation System is the ConversationSystem class. It consumes several interfaces, then returns responses for topics as the user asks about them.
 
# InputSet and OutputSet #
ConversationSystem consumes InputSets and produces OutputSets. An InputSet is consists of a topic and a set of variables pertinent to the conversation. These variables are used to appropriately pick responses. OutputSet consists of the text body of the response and a list of non-response notifications such as item receipt notifications or quest updates.

# Topic Sources #

One of the interfaces consumed by ConversationSystem is an IEnumerable of ITopicSource, these provide the list of topics to select from. The *first* available response will always be chosen.

A default implementation is provided as CsvTopicSource, which reads Topics from CSV files provided via a stream. The expected CSV format is straightforward, one response to a row. The first column should be the name of the topic the response belongs to, the second column should be the body of the response itself, the third column should be an AvailabilityExpression for the response (or empty), the fourth column should be the ResponseActionScript for the response (or empty).

# Availability Expression Evaluators #

ConversationSystem also consumes IAvailabilityExpressionEvaluators, these are used to evaluate the AvailabilityExpression on each Response to determine which ones are available for the provided InputSet. If any of the IAvailabilityExpressionEvaluators evaluates an AvailabilityExpression as true, then the Response is considered valid.

A default implementation is provided as VariableAvailabilityExpressionEvaluator, which performs basic boolean logic and checks variable values. The syntax used by VariableAvailabilityExpressionEvaluator is explained below.

    variable_name="value" || !other_variable_name="other_value" && some_variable_name=another_variable_name || "some_value"="yet_another_value"

    || - logical OR
    
    && - logical AND
    
    ! - logical NOT

    = - compare two values, either side can be a static value in quotes (") or a variable name (no quotes)

# Post Processors #

After a response has been selected ConversationSystem calls any IPostProcessors it has been provided. These are given both the InputSet and OutputSet and allowed to make any modifications to the response they wish. A UnmParserPostProcessor is provided along with GCS that utilizes the UNM NameParser to generate responses based on UNM markup in the body of Responses. This is a recommended post processor to use, as it is the key to a robust conversation system.

# Response Action Processors #

Lastly the ConversationSystem calls any IResponseActionProcessors it has been provided. These are passed the InputSet and OutputSet (along with any modifications made by IPostProcessors) along with the ResponseActionScript from the chosen Response. Response actions processors are useful to update quest states, gift items, update journal entries, unlock doors, or otherwise modify the world.

IResponseActionProcessors could also modify the response in the OutputSet, but in general that is better handled by a IPostProcessor.

# Best Practices #

Sub pattern tags from UNM make good choices for response bodies, in combination with the UnmParserPostProcessor they allow for varied responses from a single Response object.

Order is important for Responses in a topic. The first available response will always be chosen, any "catch all" or general response should be at the bottom, with the most specific or important response at the top.

If a IResponseActionProcessors gives an item or modifies the world it is a good idea to add a notification to the Notifications list of the OutputSet to let the player know.