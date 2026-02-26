namespace Task_20260225.Common.Utils;

public enum ErrorCode
{
    ResultOk = 0,
    InvalidCommand = 101,
    InvalidQuery = 102,
    InvalidRequest = 103,

    UploadEmployeeInfoFileIsNull = 151,
    UploadEmployeeInfoWrongFileType = 152,
    UploadEmployeeInfoTextEmpty = 153,
    UploadEmployeeInfoInvalidJson = 154,
    UploadEmployeeInfoInvalidCsv = 155,
    UploadEmployeeInfoInvalidJsonWithText = 156,
    UploadEmployeeInfoInvalidCsvWithText = 157,

    GetEmployeeByNameEmptyName = 171,
    
    GetEmployeeWrongPageOrPageSize = 181,
    
    ContactInvalidEmailFormat = 191,
    ContactInvalidDateFormat = 192,
}
    
