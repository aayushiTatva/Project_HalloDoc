using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class TransferNotesModel
    {
        public int RequestStatusLogId { get; set; }
        public int RequestId { get; set; }
        public int? PhysicianId { get; set; }
        public int? TransToPhysicianId { get; set; }
        public string? Admin { get; set; }
        public string? Physician { get; set; }
        public string? Notes { get; set; }
        public BitArray? TransToAdmin { get; set; }
        public string TransToPhysician { get; set; }
        public short Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TransferNotes => $"{Admin} transferred <b> to {Physician} </b> to <b> {TransToPhysician} </b> on {CreatedDate}: <b>{Notes}</b>";
    }
}
