using Church.Ysa.Domain;
using FluentAssertions;
using LaytonYSAClerk.WebTool.Services;

namespace WebTool.Tests.Unit;

public class When_MemberGroupFactory
{
    [Fact]
    public void IsCalled_WithoutOneGender_ShouldReturnValidGroups()
    {
        // Arrange
        List<SimpleMember> members =
        [
            new()
            {
                Name = "Kayden",
                Gender = Gender.Male
            },
            new()
            {
                Name = "Josh",
                Gender = Gender.Male
            }
        ];
        List<SimpleMember> fellowshipMembers = [
            new()
            {
                Name = "Bob",
                Gender = Gender.Male
            }
        ];

        // Act
        var sut = new MemberGroupFactory(members, fellowshipMembers);
        var result = sut.CreateGroups().ToList();

        // Assert
        result.Count.Should().Be(1);
        result[0].GroupMembers.Count().Should().Be(2);
        result[0].GroupMembers.Should().Contain(members);
    }
    
    
    [Fact]
    public void IsCalled_ShouldReturnWithoutDuplicates()
    {
        // Arrange
        List<SimpleMember> members =
        [
            new()
            {
                Name = "Kayden",
                Gender = Gender.Male
            },
            new()
            {
                Name = "Josh",
                Gender = Gender.Male
            }
        ];
        List<SimpleMember> fellowshipMembers = [
            new()
            {
                Name = "Bob",
                Gender = Gender.Male
            }
        ];

        // Act
        var sut = new MemberGroupFactory(members, fellowshipMembers);
        var result = sut.CreateGroups().ToList();

        // Assert
        result.Count.Should().Be(1);
        result[0].ObjectiveMembers?.Count().Should().Be(1);
    }
}