using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Lab2;

namespace Lab2_test
{
    public class UnitTest1
    {
        [Fact]
        public void Fruit_ProperFormat_ShouldStartWithFruit()
        {
            var fruit = new Fruit { Name = "Apple" };
            var result = fruit.ToString();
            Assert.StartsWith("Fruit", result);
        }

        [Fact]
        public async Task Fruit_ProperFormat_ShouldMatchExpectedFormatAsync()
        {
            USDCourse.Current = await USDCourse.GetUsdCourseAsync();
            var fruit = new Fruit
            {
                Name = "Apple",
                IsSweet = true,
                Price = 5.50m
            };
            var expectedFormat = $"Fruit: Name=Apple, IsSweet=True, Price=5,50 z³, PriceUSD={USDCourse.FormatUsdPrice(5.50m/USDCourse.Current)} $";

            var result = fruit.ToString();

            Assert.Equal(expectedFormat, result);
        }

        [Fact]
        public async Task Fruit_Create()
        {
            List<Fruit> fruits = [];
            for (int i = 0; i < 20; i++)
            {
                fruits.Add(Fruit.Create());
            }
            string name = fruits[0].Name;
            int c = fruits.SkipWhile(f => f.Name == name).Count();
            Assert.True(c>0);
        }
    }
}