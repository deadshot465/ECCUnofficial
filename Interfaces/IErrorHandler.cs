using System;
using System.Threading.Tasks;

namespace ECCUnofficial.Interfaces
{
    public interface IErrorHandler
    {
        public Task HandleErrorAsync(Enum error);
    }
}
