namespace ABCRetail.Functions.Models;

    //This is a default class im scared to delete because its tied to the error view


    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

