using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Test
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = 
                new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact] //De muestra, se reemplaza por el de abajo porque si dejamos el mock x default nunca llega a la condici?n que esto deber?a comprobar
        public void ReferYoungApplicationsFirstVersion() //Se reemplaza por el de abajo
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications() //M?todos
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
               new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true); //Con esto seteamos un comportamiento personalizado en el Moq. Creo que ac? ser?a: Cuando llamamos a IsValid con "x" como par?metro, que retorne true.
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true); //ac? ser?a: Cuando llamamos a IsValid con un string como par?metro, que retorne true.
            //mockValidator.Setup(
            //        x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))) //ac? ser?a: Cuando llamamos a IsValid con un string como par?metro y el n?mero(que en este caso es un string) comienza con "y", que retorne true.
            //    .Returns(true);

            //mockValidator.Setup(
            //        x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))) //ac? ser?a: Cuando llamamos a IsValid con un string como par?metro y el n?mero(que en este caso es un string) est? entre "a" y "z", que retorne true.
            //    .Returns(true);

            //mockValidator.Setup(
            //        x => x.IsValid(It.IsIn<string>("a", "z", "y", "x"))) //ac? ser?a: Cuando llamamos a IsValid con un string como par?metro y el n?mero(que en este caso es un string) est? en la lista que definimos, que retorne true.
            //    .Returns(true);

            mockValidator.Setup(
                    x => x.IsValid(It.IsRegex("[a-z]"))) //ac? ser?a: Cuando llamamos a IsValid con un string como par?metro y el n?mero(que en este caso es un string) cumple con la expresi?n regular, que retorne true.
                .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "x"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);

        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplication() //MockBehavior.Strict
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>(/*MockBehavior.Strict*/); //Ahora falla si hay un m?todo que se llama que est? sin especificar

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);

        }

        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo() //trabajando con out
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true; //Este valor es el que queremos que tome el out

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid)); //ac? seteamos el valor del out en el m?todo is valid

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { 
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }


        [Fact]
        public void ReferWhenLicenseKeyExpired() //Propiedades
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED"); //Como esto es una propiedad le decimos como queremos que retorne los valores.
            mockValidator.Setup(x => x.LicenseKey).Returns(GetLicenseKeyExpiryString());

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpiryString()
        {
            return "EXPIRED"; //Ac? podemos leer por ejemplo de la api de un proveedor.
        }
    }
}
