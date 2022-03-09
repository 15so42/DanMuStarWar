using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorType
{
   Success,
   Failure,
}
public class ErrorCode 
{
   public ErrorType code;
   public string errorStr;

   public ErrorCode(ErrorType code, string errorStr)
   {
      this.code = code;
      this.errorStr = errorStr;
   }
}
