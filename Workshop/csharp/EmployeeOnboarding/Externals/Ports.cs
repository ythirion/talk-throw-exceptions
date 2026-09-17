using CSharpFunctionalExtensions;
using EmployeeOnboarding.Models;

namespace EmployeeOnboarding.Externals;

public interface IEmployeeRepository
{
   Result<Employee, Error> Register(AcceptedOffer offer);
}

public interface IHrSystem
{
    Result<Contract, Error> GenerateContract(Employee employee);
}

public interface IItProvisioning
{
    Result<Account, Error> ProvisionAccount(Contract contract);
}

public interface IPayroll
{
    Result<OnboardingResult, Error> Enroll(Account account);
}