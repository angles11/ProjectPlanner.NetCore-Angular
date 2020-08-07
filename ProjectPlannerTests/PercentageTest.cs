using NUnit.Framework;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System.Collections.Generic;

namespace ProjectPlannerTests
{
    [TestFixture]
    public class PercentageTests
    {

        public ICollection<Todo> CreateList(int completedTodos, int totalTodos)
        {
            var todos = new List<Todo>();

                for (int i = 0; i < completedTodos; i++)
                {
                    todos.Add(new Todo { Status = "Completed" });
                }


            for (int i = 0; i < totalTodos - completedTodos; i++)
            {
                todos.Add(new Todo { Status = "Pending" });
            }

            return todos;
        }

        [Test]
        public void AllTodosCompleted()
        {
            var todos = CreateList(2, 2);

            var result = todos.CalculatePercentage();

            Assert.AreEqual(100, result);
        }

        [Test]
        public void HalfTodosCompleted()
        {
            var todos = CreateList(2, 4);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(50, result);
        }

        [Test]
        public void ThirdPartCompleted()
        {
            var todos = CreateList(3, 9);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(33.33, result);
        }

        [Test]
        public void FifthPartCompleted()
        {
            var todos = CreateList(2, 10);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(20, result);
        }

        [Test]
        public void SixthPartCompleted()
        {
            var todos = CreateList(1, 6);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(16.67, result);
        }

        [Test]
        public void SeventhPartCompleted()
        {
            var todos = CreateList(2, 14);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(14.29, result);
        }

        [Test]
        public void NoneCompleted()
        {
            var todos = CreateList(0, 10);
            var result = todos.CalculatePercentage();

            Assert.AreEqual(0, result);
        }
    }
}