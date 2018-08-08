using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public string SrcSystemRoomId { get; set; }
        public string SrcSystemName { get; set; }
        public string RoomName { get; set; }

        public int ResidentGroupId { get; set; }
        public ResidentGroup ResidentGroup { get; set; }
    }
}
