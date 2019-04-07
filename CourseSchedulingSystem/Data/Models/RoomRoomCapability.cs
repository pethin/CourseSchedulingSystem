﻿using System;

namespace CourseSchedulingSystem.Data.Models
{
    public class RoomRoomCapability
    {
        public Guid RoomCapabilityId { get; set; }
        public virtual RoomCapability RoomCapability { get; set; }

        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}