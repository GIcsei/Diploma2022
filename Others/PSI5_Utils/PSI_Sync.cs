using System.Collections.Generic;

namespace PSI_Checker_2p0.PSI5_Utils
{
    sealed class PSI_Sync : DataContainer
    {
        public const string ContainerName = "PSI_SYNC";
        public PSI_Sync() : base(ContainerName)
        {
            RestrictedNames = new List<string>
            {

            };
        }

        public override void CalculateRestrictedFields()
        {
            base.CalculateRestrictedFields();
        }
        // TODO
        /*
         Feszültség:
            V_Trig: Mikortól szinkron pulzus: Állítható érték
            V_t2: Beállási idő "alja"
            Beállás definíciója kérdéses... (Sync pulse width)
            Trigger point-hoz időbélyeg!
            Abszolút: 0-ik mintától dt-vel, relatív: t0-t_Trig-től dt-vel
            Minimális várakozási idő a sync pulse után

            RSET: Volt-e reset a kommunikáció során. --> Nem kritikus egyelőre

            Szoftverbe bekerülhető: 
            Max. volt. cap: 11-ről 16.5-re és aztán vissza 0-ra.
            Initphase1 hosszának megmérése --> PowerOn és első válasz ideje.
            t_set mérése (beállási idő)
            50-150ms: peak-to-peak
            t_set és init1 vége közti: peak-to-peak
            PowerOn: Limit megadás
            Limit ideje: 5ms
            Min-max különbség mellé: Medián

            InitPhase 2: Block ID és status data
            InitPhase 3      
         
         */
    }
}