namespace Test_Api.Dtos.Request
{
    public record FilterdataRequest(
    
        string? name, decimal? minprice , decimal? maxprice , int? catedoryid , int? instractorid 
    );
}
