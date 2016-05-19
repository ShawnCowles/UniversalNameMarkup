using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using UNM.GCS.Implementation;

namespace UNM.GCS.Test
{
    [TestFixture]
    public class VariableAvailabilityExpressionEvaluatorTest
    {
        private readonly Fixture _fixture = new Fixture();

        private VariableAvailabilityExpressionEvaluator _evaluator;

        [SetUp]
        public void Setup()
        {
            _evaluator = new VariableAvailabilityExpressionEvaluator();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Evaluate_handles_basic_variable_check(bool match)
        {
            var variableName = _fixture.Create<string>();
            var variableValue = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();

            if(match)
            {
                variables.Add(variableName, variableValue);
            }
            else
            {
                variables.Add(variableName, _fixture.Create<string>());
            }

            var expression = string.Format("{0}=\"{1}\"",
                variableName, variableValue);

            Assert.That(_evaluator.Evaluate(expression, variables), Is.EqualTo(match));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Evaluate_handles_variable_to_variable_check(bool match)
        {
            var variableAName = _fixture.Create<string>();
            var variableBName = _fixture.Create<string>();
            var variableValue = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();

            variables.Add(variableAName, variableValue);

            if (match)
            {
                variables.Add(variableBName, variableValue);
            }
            else
            {
                variables.Add(variableBName, _fixture.Create<string>());
            }

            var expression = string.Format("{0}={1}",
                variableAName, variableBName);

            Assert.That(_evaluator.Evaluate(expression, variables), Is.EqualTo(match));
        }

        [Test]
        public void Evaluate_errors_matching_value_to_value()
        {
            var valueA = _fixture.Create<string>();
            var valueB = _fixture.Create<string>();
            
            var variables = new Dictionary<string, string>();
            
            var expression = string.Format("\"{0}\"=\"{1}\"",
                valueA, valueB);

            Assert.Throws<ExpressionParseException>(
                () => _evaluator.Evaluate(expression, variables));
        }

        [Test]
        public void Evaluate_errors_when_missing_left_hand_value_to_equals()
        {
            var value = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();
            
            var expression = string.Format("=\"{0}\"",
                value);

            Assert.Throws<ExpressionParseException>(
                () => _evaluator.Evaluate(expression, variables));
        }

        [Test]
        public void Evaluate_errors_when_missing_right_hand_value_to_equals()
        {
            var variableName = _fixture.Create<string>();
            var variableValue = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();

            variables.Add(variableName, _fixture.Create<string>());
            
            var expression = string.Format("{0}=",
                variableName);

            Assert.Throws<ExpressionParseException>(
                () => _evaluator.Evaluate(expression, variables));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Evaluate_handles_AND_statements(bool a, bool b)
        {
            var expected = a && b;

            var variableAName = _fixture.Create<string>();
            var variableBName = _fixture.Create<string>();
            var variableCName = _fixture.Create<string>();
            var valueA = _fixture.Create<string>();
            var valueB = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();

            variables.Add(variableAName, valueA);

            if (a)
            {
                variables.Add(variableBName, valueA);
            }
            else
            {
                variables.Add(variableBName, _fixture.Create<string>());
            }

            if (b)
            {
                variables.Add(variableCName, valueB);
            }
            else
            {
                variables.Add(variableCName, _fixture.Create<string>());
            }

            var expression = string.Format("{0}={1}&&{2}=\"{3}\"",
                variableAName, variableBName, variableCName, valueB);

            Assert.That(_evaluator.Evaluate(expression, variables), Is.EqualTo(expected));
        }

        [Test]
        public void Evaluate_handles_multiple_AND_statements()
        {
            var variableA = _fixture.Create<string>();
            var variableB = _fixture.Create<string>();
            var valueA = _fixture.Create<string>();
            var valueB = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();
            variables.Add(variableA, valueA);
            variables.Add(variableB, valueB);

            var expression = string.Format(
                "{0}=\"{1}\" && {2}=\"{3}\" && {0}=\"{1}\" && {2}=\"{3}\"",
                variableA,
                valueA,
                variableB,
                valueB);

            Assert.True(_evaluator.Evaluate(expression, variables));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Evaluate_handles_OR_statements(bool a, bool b)
        {
            var expected = a || b;

            var variableAName = _fixture.Create<string>();
            var variableBName = _fixture.Create<string>();
            var variableCName = _fixture.Create<string>();
            var valueA = _fixture.Create<string>();
            var valueB = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();

            variables.Add(variableAName, valueA);

            if (a)
            {
                variables.Add(variableBName, valueA);
            }
            else
            {
                variables.Add(variableBName, _fixture.Create<string>());
            }

            if (b)
            {
                variables.Add(variableCName, valueB);
            }
            else
            {
                variables.Add(variableCName, _fixture.Create<string>());
            }

            var expression = string.Format("{0}={1} || {2}=\"{3}\"",
                variableAName, variableBName, variableCName, valueB);

            Assert.That(_evaluator.Evaluate(expression, variables), Is.EqualTo(expected));
        }

        [Test]
        public void Evaluate_handles_multiple_AND_and_OR_statements()
        {
            var variableA = _fixture.Create<string>();
            var variableB = _fixture.Create<string>();
            var valueA = _fixture.Create<string>();
            var valueB = _fixture.Create<string>();

            var variables = new Dictionary<string, string>();
            variables.Add(variableA, valueA);
            variables.Add(variableB, valueB);

            var expression = string.Format(
                "{0}=\"{1}\" || {2}=\"{3}\" && {0}=\"{1}\" && {2}=\"{3}\"",
                variableA,
                valueA,
                variableB,
                valueB);

            Assert.True(_evaluator.Evaluate(expression, variables));
        }

        [Test]
        public void Evaluate_passes_empty_expressions()
        {
            Assert.True(_evaluator.Evaluate("", new Dictionary<string, string>()));
        }
    }
}
