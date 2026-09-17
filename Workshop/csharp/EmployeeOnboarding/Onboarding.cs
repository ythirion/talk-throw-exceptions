using CSharpFunctionalExtensions;
using EmployeeOnboarding.Externals;
using EmployeeOnboarding.Models;

namespace EmployeeOnboarding;

public class Onboarding(IEmployeeRepository employees, IHrSystem hr, IItProvisioning it, IPayroll payroll)
{
    public Result<OnboardingResult, Error> OnboardNewHire(AcceptedOffer offer) 
        => employees.Register(offer)
            .Bind(hr.GenerateContract)
            .Bind(it.ProvisionAccount)
            .Bind(payroll.Enroll);
}