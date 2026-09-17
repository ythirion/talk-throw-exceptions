using EmployeeOnboarding.Externals;
using EmployeeOnboarding.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace EmployeeOnboarding.Tests;

public class OnboardingTests
{
    // ── Test data ─────────────────────────────────────────────────────────────
    private static readonly AcceptedOffer Offer = new()
        { Name = "Alice", Email = "alice@corp.com", Department = "Engineering", StartDate = new DateTime(2024, 1, 15) };

    private static readonly Employee RegisteredEmployee = new() { Id = 1, Name = "Alice", Email = "alice@corp.com" };

    private static readonly Contract GeneratedContract = new()
        { Id = 100, EmployeeId = 1, StartDate = new DateTime(2024, 1, 15) };

    private static readonly Account ProvisionedAccount = new() { EmployeeId = 1, Login = "alice.corp" };

    private static readonly OnboardingResult EnrollmentResult = new()
        { EmployeeId = 1, Login = "alice.corp", EnrolledAt = new DateTime(2024, 1, 15) };

    private readonly IEmployeeRepository _employees = Substitute.For<IEmployeeRepository>();
    private readonly IHrSystem _hr = Substitute.For<IHrSystem>();
    private readonly IItProvisioning _it = Substitute.For<IItProvisioning>();
    private readonly Onboarding _onboarding;
    private readonly IPayroll _payroll = Substitute.For<IPayroll>();

    public OnboardingTests()
    {
        _onboarding = new Onboarding(_employees, _hr, _it, _payroll);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void Should_return_enrollment_when_all_steps_succeed()
    {
        _employees.Register(Offer).Returns(RegisteredEmployee);
        _hr.GenerateContract(RegisteredEmployee).Returns(GeneratedContract);
        _it.ProvisionAccount(GeneratedContract).Returns(ProvisionedAccount);
        _payroll.Enroll(ProvisionedAccount).Returns(EnrollmentResult);

        var result = _onboarding.OnboardNewHire(Offer);

        result.Should()
            .SucceedWith(EnrollmentResult);
    }

    // ── Failure paths ─────────────────────────────────────────────────────────

    [Fact]
    public void Should_return_a_duplicated_employee_error_when_employee_registration_fails()
    {
        var duplicateEmployeeError = new Error("Duplicate employee record");
        _employees.Register(Offer)
            .Returns(duplicateEmployeeError);

         _onboarding.OnboardNewHire(Offer)
             .Should()
             .FailWith(duplicateEmployeeError);
    }

    [Fact]
    public void Should_return_a_contract_error_when_contract_generation_fails()
    {
        _employees.Register(Offer).Returns(RegisteredEmployee);
        var contractGenerationError = new Error("Missing salary band");
        _hr.GenerateContract(RegisteredEmployee)
            .Returns(contractGenerationError);

         _onboarding.OnboardNewHire(Offer)
             .Should()
             .FailWith(contractGenerationError);
    }

    [Fact]
    public void Should_return_an_account_error_when_account_provisioning_fails()
    {
        _employees.Register(Offer).Returns(RegisteredEmployee);
        _hr.GenerateContract(RegisteredEmployee).Returns(GeneratedContract);
        
        var accountProvisioningError = new Error("Login already taken");
        _it.ProvisionAccount(GeneratedContract)
            .Returns(accountProvisioningError);

        _onboarding.OnboardNewHire(Offer)
            .Should()
            .FailWith(accountProvisioningError);
    }

    [Fact]
    public void Should_return_a_payroll_enrollment_error()
    {
        _employees.Register(Offer).Returns(RegisteredEmployee);
        _hr.GenerateContract(RegisteredEmployee).Returns(GeneratedContract);
        _it.ProvisionAccount(GeneratedContract).Returns(ProvisionedAccount);
        
        var enrollmentError = new Error("Payroll system unavailable");
        _payroll.Enroll(ProvisionedAccount).Returns(enrollmentError);

        _onboarding.OnboardNewHire(Offer)
            .Should()
            .FailWith(enrollmentError);
    }
}