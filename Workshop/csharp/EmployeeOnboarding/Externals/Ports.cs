using CSharpFunctionalExtensions;
using EmployeeOnboarding.Models;

namespace EmployeeOnboarding.Externals;

public interface IEmployeeRepository
{
    Employee Register(AcceptedOffer offer);
}

public interface IHrSystem
{
    Contract GenerateContract(Employee employee);
}

public interface IItProvisioning
{
    Result<Account, Error> ProvisionAccount(Contract contract);
}

public interface IPayroll
{
    Result<OnboardingResult, Error> Enroll(Account account);
}