using Moq;
using Xunit;

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

        /*[Fact] //De muestra, se reemplaza por el de abajo porque si dejamos el mock x default nunca llega a la condición que esto debería comprobar
        public void ReferYoungApplicationsFirstVersion() //Se reemplaza por el de abajo
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }*/

        [Fact]
        public void ReferYoungApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.DefaultValue = DefaultValue.Mock; //Esto hace que se haga un mock automático de todas las clases que dependen de lo que estás moqueando
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications() //Métodos
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
               new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true); //Con esto seteamos un comportamiento personalizado en el Moq. Creo que acá sería: Cuando llamamos a IsValid con "x" como parámetro, que retorne true.
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true); //acá sería: Cuando llamamos a IsValid con un string como parámetro, que retorne true.
            //mockValidator.Setup(
            //        x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))) //acá sería: Cuando llamamos a IsValid con un string como parámetro y el número(que en este caso es un string) comienza con "y", que retorne true.
            //    .Returns(true);

            //mockValidator.Setup(
            //        x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))) //acá sería: Cuando llamamos a IsValid con un string como parámetro y el número(que en este caso es un string) está entre "a" y "z", que retorne true.
            //    .Returns(true);

            //mockValidator.Setup(
            //        x => x.IsValid(It.IsIn<string>("a", "z", "y", "x"))) //acá sería: Cuando llamamos a IsValid con un string como parámetro y el número(que en este caso es un string) está en la lista que definimos, que retorne true.
            //    .Returns(true);

            mockValidator.Setup(
                    x => x.IsValid(It.IsRegex("[a-z]"))) //acá sería: Cuando llamamos a IsValid con un string como parámetro y el número(que en este caso es un string) cumple con la expresión regular, que retorne true.
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
                new Mock<IFrequentFlyerNumberValidator>(/*MockBehavior.Strict*/); //Ahora falla si hay un método que se llama que está sin especificar

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);

        }

        /*[Fact]
        public void DeclineLowIncomeApplicationsOutDemo() //trabajando con out
        {

            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true; //Este valor es el que queremos que tome el out

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid)); //acá seteamos el valor del out en el método is valid

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { 
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }*/


        [Fact]
        public void ReferWhenLicenseKeyExpired() //Propiedades
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED"); //Como esto es una propiedad le decimos como queremos que retorne los valores.


            // Moqueando toda la herencia a mano
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");

            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);


            //Pero la forma más rápida
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");

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
            return "EXPIRED"; //Acá podemos leer por ejemplo de la api de un proveedor.
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode); //Si no hacemos esto no va a recordar los cambios en el moq ***

            //Otra alternativa para guardar el estado de todas las propiedades
            mockValidator.SetupAllProperties(); // Siemnpre poner lo mas arriba posible, si no overridea todos los setups.

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode); //*** si no ponemos eso acá va a fallar pq trae el valor x defecto q es Quick
        }
    }
}
