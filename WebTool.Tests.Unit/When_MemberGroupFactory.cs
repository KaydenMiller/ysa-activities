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

    [Fact]
    public void When_MultipleGroups_WithEvenFellowship()
    {
        List<SimpleMember> members =
        [
            new()
            {
                Name = "1",
                Gender = Gender.Male
            },
            new()
            {
                Name = "2",
                Gender = Gender.Male
            },
            new()
            {
                Name = "3",
                Gender = Gender.Male
            },
            new()
            {
                Name = "4",
                Gender = Gender.Male
            }
        ];
        List<SimpleMember> fellowshipMembers = [
            new()
            {
                Name = "1",
                Gender = Gender.Male
            },
            new()
            {
                Name = "2",
                Gender = Gender.Male
            },
            new()
            {
                Name = "3",
                Gender = Gender.Male
            },
            new()
            {
                Name = "4",
                Gender = Gender.Male
            }
        ];

        var sut = new MemberGroupFactory(members, fellowshipMembers);
        var groups = sut.CreateGroups().ToList();

        groups.Count.Should().Be(2);
        groups[0].GroupMembers.Count().Should().Be(2);
        groups[0].ObjectiveMembers!.Count().Should().Be(2);
        groups[1].GroupMembers.Count().Should().Be(2);
        groups[1].ObjectiveMembers!.Count().Should().Be(2);
    }
}