using Framework.WebUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Interfaces
{
    public interface IValidate<T>
    {
        ValidationResult Validate(T model);
    }
}
