using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.Analyzer;
using PSI_Checker_2p0.Sensor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Checker
{
    public class CheckerMeasurement<T> : BaseMeasurement<T>
        where T : BaseResultHandler, new()
    {
        private readonly Pattern CurrentPattern;

        public CheckerMeasurement(ISensor sensor, Pattern pattern) : base(sensor)
        {
            CurrentPattern = pattern;
        }

        private async Task Analyze(double[] analogVoltageData, double[] analogCurrenData)
        {
            foreach (var analyzer in AnalyzerBuilder.GetAnalyzers(Sensor.Protocol, ResultData))
            {
                await analyzer.Analyze(analogVoltageData, analogCurrenData);
            }
        }

        /// <summary>
        /// Perform measurement with the given scope and controller.
        /// It is the duty of the previous func. calls to make sure,
        /// the devices are set up correctly.
        /// </summary>
        /// <param name="scope">The scope-control thread</param>
        /// <param name="controller">The ECU-control thread</param>
        /// <returns></returns>
        public override async Task<T> Measure(IScopeThread scope, IControllerThread controller)
        {
            /*Mérése menete:
             1. Pattern beállítása és leküldése az FPGA-nak.
             2. Protocol beállítása --> Default
             3. Elindítjuk a szkópot
             4. Kiadjuk a tápot az ECUn
             5. Mérés végeztével táp leállítás
             6. Adatok visszatöltése
             7. Analizálás
            */
            Scope = scope;
            Controller = controller;
            await SelfLock.WaitAsync();
            try
            {
                /*
                await SetProtocol();
                await SendPattern();
                if (scope.IsRunning)
                {
                    await scope.StopThread();
                }
                await scope.StartThread();
                await controller.StartThread();
                await Task.Delay(1000); //Maximum length of the measurement
                await scope.StopThread();
                await controller.StopThread();*/
                foreach (var data in Scope.FileSaver.LoadAllDataForChannel(0).Zip(
                    Scope.FileSaver.LoadAllDataForChannel(1), Tuple.Create))
                {
                    await Analyze(data.Item1, data.Item2);
                }
            }
            catch { }
            finally { SelfLock.Release(); }
            return ResultData;
        }

        /// <summary>
        /// Sets the given protocol setting on the sensor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task SetProtocol()
        {
            //DIService.Logger.Log("Protocol was set!");
            await Controller.SetProtocol(Sensor.Protocol);
        }

        /// <summary>
        /// Send the patterns to the <see cref="ControllerDeviceThread"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task SendPattern()
        {
            //DIService.Logger.Log("Pattern was set in the Hardware!");
            await Controller.SetPattern(CurrentPattern);
        }
    }
}
