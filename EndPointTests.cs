using EndPointManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EndPointTests
{
    [TestClass]
    public class EndPointTests
    {
        private BL _bl;

        public EndPointTests()
        {
            _bl = new BL();
        }
        [TestMethod]
        [DataRow(null, null, 0, null, 0)]
        [DataRow("SN002", null, 0, null, 0)]
        [DataRow("SN002", "INVALID", 0, null, 0)]
        [DataRow("SN002", "NSX1P3W", 0, null, 0)]
        [DataRow("SN002", "NSX1P3W", -1, null, 0)]
        [DataRow("SN002", "NSX1P3W", 0, null, 0)]
        [DataRow("SN002", "NSX1P3W", 0, "VERSION", 0)]
        [DataRow("SN002", "NSX1P3W", 0, "VERSION", -1)]
        [DataRow("SN00080007", "NSX1P3W", 0, "VERSION", 1)]

        // Como o preenchimento no console é sequencial, os testes foram criados seguindo esta mesma lógica, onde o primeiro parâmetro
        // inválido será retornado como erro. Alterar esta sequência afeta os testes.
        public void Insert_Validating_Values(string serialNumber, string meterModelId, int meterNumber, string meterFirmwareVersion, int switchState)
        {
            Enum.TryParse(meterModelId, out Models modelId);

            try
            {
                _bl.InsertEndPoint(serialNumber, meterModelId, meterNumber, meterFirmwareVersion, switchState);

                if (string.IsNullOrEmpty(serialNumber) || 
                    !Enum.IsDefined(typeof(Models), modelId) ||
                    meterNumber < 0 ||
                    string.IsNullOrEmpty(meterFirmwareVersion) ||
                    !Enum.IsDefined(typeof(States), switchState))
                    Assert.Fail("Insert accepting invalid values");
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(serialNumber))
                {
                    if (ex.Message != "Serial Number not informed")
                        Assert.IsFalse(true, "Serial Number accepting empty value");
                }
                else if (serialNumber.Length > 20)
                {
                    if (ex.Message != "Serial Number too long (Max 20 characters)")
                        Assert.IsFalse(true, "Serial Number accepting more than 20 characters");
                }
                else if (!Enum.IsDefined(typeof(Models), modelId))
                {
                    if (ex.Message != "Model invalid")
                        Assert.IsFalse(true, "Meter Model accepting non registered values");
                }
                else if (meterNumber < 0)
                {
                    if (ex.Message != "Meter Number cannot be negative")
                        Assert.IsFalse(true, "Meter Number accepting negative value");
                }
                else if (string.IsNullOrEmpty(meterFirmwareVersion))
                {
                    if (ex.Message != "Firmware Version not informed")
                        Assert.IsFalse(true, "Meter Firmware Version accepting empty value");
                }
                else if (meterFirmwareVersion.Length > 20)
                {
                    if (ex.Message != "Firmware Version too long (Max 20 characters)")
                        Assert.IsFalse(true, "Firmware Version accepting more than 20 characters");
                }
                else if (!Enum.IsDefined(typeof(States), switchState))
                {
                    if (ex.Message != "Invalid Switch State")
                        Assert.IsFalse(true, "Switch State accepting invalid values");
                }

            }
        }

        [TestMethod]
        [DataRow("SN00000007", "NSX1P3W", 0, "VERSION", 1)]
        [DataRow("SN12345678", "NSX1P3W", 0, "VERSION", 1)]
        public void Insert_Serial_Duplicated(string serialNumber, string meterModelId, int meterNumber, string meterFirmwareVersion, int switchState)
        {
            EndPoint endPoint;
            // Utilizando a lista de mock test para agilizar
            _bl.GenerateMockList();

            try
            {
                endPoint = _bl.getEndPoint(serialNumber,true);
                int ret = _bl.InsertEndPoint(serialNumber, meterModelId, meterNumber, meterFirmwareVersion, switchState);

                if (endPoint != null && ret > 0)
                    Assert.IsFalse(true, "Inserting duplicate Serial Number");
            }
            catch (Exception ex)
            {
                if (ex.Message != "Serial Number already in use")
                    Assert.IsFalse(true, "Unhandled error");
            }
            
        }

        [TestMethod]
        [DataRow("SN12345678", 99)]
        [DataRow("SN12345678", 1)]
        [DataRow("SN00000007", -1)]
        [DataRow("SN00000007", 99)]
        [DataRow("SN00000007", 0)]
        public void Edit_Invalid_Parameters(string serialNumber, int switchState)
        {
            EndPoint endPoint = null;

            try
            {
                // Utilizando a lista de mock test para agilizar
                _bl.GenerateMockList();

                endPoint = _bl.getEndPoint(serialNumber, true);

                _bl.EditEndPoint(serialNumber, switchState);
            }
            catch (Exception ex)
            {
                if (endPoint == null)
                {
                    if (ex.Message != "Serial Number not found")
                        Assert.IsFalse(true, "Expected error for not founding object in the list");
                }
                else if (!Enum.IsDefined(typeof(States), switchState))
                {
                    if (ex.Message != "Invalid Switch State")
                        Assert.IsFalse(true, "Expected error for informing invalid Switch State");
                }
            }
        }

        [TestMethod]
        [DataRow("SN12345678")]
        [DataRow("SN00000007")]
        public void Delete_Invalid_Parameters(string serialNumber)
        {
            EndPoint endPoint = null;

            try
            {
                // Utilizando a lista de mock test para agilizar
                _bl.GenerateMockList();

                endPoint = _bl.getEndPoint(serialNumber, true);

                _bl.DeleteEndPoint(serialNumber);
            }
            catch (Exception ex)
            {
                if (endPoint == null)
                {
                    if (ex.Message != "Serial Number not found")
                        Assert.IsFalse(true, "Expected error for not founding object in the list");
                }
            }
        }

        [TestMethod]
        [DataRow("SN12345678")]
        [DataRow("SN00000007")]
        public void Find_Invalid_Parameters(string serialNumber)
        {
            EndPoint endPoint = null;

            try
            {
                // Utilizando a lista de mock test para agilizar
                _bl.GenerateMockList();

                endPoint = _bl.getEndPoint(serialNumber);

            }
            catch (Exception ex)
            {
                if (endPoint == null)
                {
                    if (ex.Message != "Serial Number not found")
                        Assert.IsFalse(true, "Expected error for not founding object in the list");
                }
            }
        }
    }
}