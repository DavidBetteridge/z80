using Xunit;

namespace z80vm.Tests
{
    public class LabelTests
    {
        [Fact]
        public void It_Should_Be_Possible_Define_And_Read_Back_A_Label()
        {
            var machine = new Machine();
            machine.Labels.Set("A_LABEL", 0x1234);

            Assert.Equal(0x1234, machine.Labels.Read("A_LABEL"));
        }

        [Fact]
        public void It_Should_Be_Possible_Define_And_Read_Back_Multiple_Labels()
        {
            var machine = new Machine();
            machine.Labels.Set("A_LABEL", 0x1234);
            machine.Labels.Set("ANOTHER_LABEL", 0xAAAA);

            Assert.Equal(0x1234, machine.Labels.Read("A_LABEL"));
            Assert.Equal(0xAAAA, machine.Labels.Read("ANOTHER_LABEL"));
        }

        [Fact]
        public void It_Should_Only_Be_Possible_To_Add_A_Label_Once()
        {
            var machine = new Machine();
            machine.Labels.Set("A_LABEL", 0x1234);
            var exception = Record.Exception(() => machine.Labels.Set("A_LABEL", 0xAAAA));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

        [Fact]
        public void An_Error_Should_Be_Reported_If_A_Undefined_Label_Is_Read()
        {
            var machine = new Machine();
            var exception = Record.Exception(() => machine.Labels.Read("A_LABEL"));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }
    }
}
