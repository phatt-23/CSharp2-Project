using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class Workspace : ObservableObject
    {
        private int _workspaceId;
        private string _name = string.Empty;
        private int _coworkingCenterId;
        private string _coworkingCenterDisplayName = string.Empty;
        private string _description = string.Empty;
        private decimal _pricePerHour;
        private WorkspaceStatusType _status;
        private bool _isRemoved;
        private DateTime _lastUpdated;

        public int WorkspaceId { get => _workspaceId; set => Set(ref _workspaceId, value); }
        public string Name { get => _name; set => Set(ref _name, value); }
        public string Description { get => _description; set => Set(ref _description, value); }
        public decimal PricePerHour { get => _pricePerHour; set => Set(ref _pricePerHour, value); }
        public int CoworkingCenterId { get => _coworkingCenterId; set => Set(ref _coworkingCenterId, value); }
        public string CoworkingCenterDisplayName { get => _coworkingCenterDisplayName; set => Set(ref _coworkingCenterDisplayName, value); }
        public WorkspaceStatusType Status { get => _status; set => Set(ref _status, value); }
        public bool IsRemoved { get => _isRemoved; set => Set(ref _isRemoved, value); }
        public DateTime LastUpdated { get => _lastUpdated; set => Set(ref _lastUpdated, value); }
    }

    public class WorkspaceDto
    {
        public required int WorkspaceId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal PricePerHour { get; set; }
        public required int CoworkingCenterId { get; set; }
        public required string CoworkingCenterDisplayName { get; set; }
        public required string Status { get; set; }
        public required bool IsRemoved { get; set; }
        public required DateTime LastUpdated { get; set; }
    }

    public class WorkspaceCreateDto
    {
        public required string Name { get; set; } 
        public required string Description { get; set; }
        public required int CoworkingCenterId { get; set; }
        public required decimal PricePerHour { get; set; }
        public required WorkspaceStatusType Status { get; set; }
    }

    public class WorkspaceUpdateDto
    {
        public required int WorkspaceId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int CoworkingCenterId { get; set; }
        public required decimal PricePerHour { get; set; }
        public required WorkspaceStatusType Status { get; set; }
    }

    public class WorkspacePageDto : PaginationResponseDto
    {
        public List<WorkspaceDto> Workspaces { get; set; } = null!;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WorkspaceStatusType { Available, Occupied, Maintenance }

    public class WorkspaceStatusHistoryDto
    {
        public int WorkspaceHistoryId { get; set; }
        public int StatusId { get; set; }
        public WorkspaceStatusType Status { get; set; }
        public DateTime ChangeAt { get; set; }
    }

    public class WorkspaceStatusHistory
    {
        public int WorkspaceHistoryId { get; set; }
        public int StatusId { get; set; }
        public WorkspaceStatusType Status { get; set; }
        public DateTime ChangeAt { get; set; }
    }

    public class WorkspaceStatusHistoryPageDto
    {
        public required List<WorkspaceStatusHistoryDto> Histories { get; set; }
        public required int TotalCount { get; set; }
    }

}
