using Moq;

namespace z80vm.Tests
{
    /// <summary>
    /// Inherit test classes from this one in order to get access to helper functions
    /// for creating the virtual machines
    /// </summary>
    public abstract class TestBase
    {

        /// <summary>
        /// Creates a machine using all the default validators
        /// </summary>
        /// <returns></returns>
        protected Machine CreateMachine()
        {
            return new Machine();
        }

        /// <summary>
        /// Creates a machine where all operands supplied for a command will be classed as invalid.  even for example ADD a,a
        /// </summary>
        /// <returns></returns>
        protected Machine CreateMachineWhereAllCommandsAreInvalid()
        {
            // Setup the machine so that all commands are invalid
            var machine = CreateMachine();
            var commandValidator = new Moq.Mock<ICommandValidator>();
            commandValidator.Setup(a => a.EnsureCommandIsValid(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<string>())).Throws(new System.InvalidOperationException("Oh no"));
            machine.SetCommandValidator(commandValidator.Object);

            return machine;
        }


        /// <summary>
        /// Creates a machine where regardless of the current flag state,  all conditions will evalulate to False
        /// </summary>
        /// <returns></returns>
        protected Machine CreateMachineWhereAllConditionsAreInvalid()
        {
            var machine = CreateMachine();
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            conditionValidator.Setup(v => v.IsTrue(It.IsAny<Flags>(), It.IsAny<Condition>())).Returns(false);
            machine.SetConditionValidator(conditionValidator.Object);

            return machine;
        }

        /// <summary>
        /// Creates a machine where regardless of the current flag state,  all conditions will evalulate to True
        /// </summary>
        /// <returns></returns>
        protected Machine CreateMachineWhereAllConditionsAreValid()
        {
            var machine = CreateMachine();
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            conditionValidator.Setup(v => v.IsTrue(It.IsAny<Flags>(), It.IsAny<Condition>())).Returns(true);
            machine.SetConditionValidator(conditionValidator.Object);

            return machine;
        }
    }
}
