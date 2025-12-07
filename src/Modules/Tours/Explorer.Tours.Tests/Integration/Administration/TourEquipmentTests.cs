using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration;

public class TourEquipmentTests
{
    private static Equipment CreateEquipment(long id, string name)
    {
        var eq = new Equipment(name, "Test description");

       
        typeof(Entity)
            .GetProperty("Id")!
            .SetValue(eq, id);

        return eq;
    }

        private static Tour CreateDraftTour(long authorId = 1)
        {
            return new Tour(
                name: "Test tour",
                description: "Opis ture",
                difficulty: 3,
                authorId: authorId,
                tags: new[] { "test" }
            );
        }

        [Fact]
        public void SetRequiredEquipment_sets_initial_equipment_on_empty_tour()
        {
            // Arrange
            var tour = CreateDraftTour();

            var eq1 = CreateEquipment(-1, "Ruksak");
            var eq2 = CreateEquipment(-2, "Kaciga");

            // Act
            tour.SetRequiredEquipment(new[] { eq1, eq2 });

            // Assert
            tour.Equipment.Count.ShouldBe(2);
            tour.Equipment.ShouldContain(e => e.Id == eq1.Id);
            tour.Equipment.ShouldContain(e => e.Id == eq2.Id);
        }

        [Fact]
        public void SetRequiredEquipment_copies_input_list_instead_of_sharing_reference()
        {
            // Arrange
            var tour = CreateDraftTour();

            var eq1 = CreateEquipment(-1, "Ruksak");
            var eq2 = CreateEquipment(-2, "Kaciga");

            var inputList = new List<Equipment> { eq1, eq2 };

            // Act
            tour.SetRequiredEquipment(inputList);

            // Menjamo originalnu listu POSLE poziva
            var eq3 = CreateEquipment(-3, "Lampa");
            inputList.Add(eq3);

            // Assert – tura ne sme da se promeni zbog izmene input liste
            tour.Equipment.Count.ShouldBe(2);
            tour.Equipment.ShouldNotContain(e => e.Id == eq3.Id);
        }


        [Fact]
        public void SetRequiredEquipment_replaces_existing_equipment()
        {
            // Arrange
            var tour = CreateDraftTour();

            var eq1 = CreateEquipment(-1, "Ruksak");
            var eq2 = CreateEquipment(-2, "Kaciga");
            var eq3 = CreateEquipment(-3, "Lampa");

            // prvo postavimo eq1, eq2
            tour.SetRequiredEquipment(new[] { eq1, eq2 });

            // Act – sada prosledimo SAMO eq3
            tour.SetRequiredEquipment(new[] { eq3 });

            // Assert – tura treba da ima SAMO eq3
            tour.Equipment.Count.ShouldBe(1);
            tour.Equipment.ShouldContain(e => e.Id == eq3.Id);
            tour.Equipment.ShouldNotContain(e => e.Id == eq1.Id || e.Id == eq2.Id);
        }

        [Fact]
        public void SetRequiredEquipment_allows_empty_list_and_clears_equipment()
        {
            // Arrange
            var tour = CreateDraftTour();

            var eq1 = CreateEquipment(-1, "Ruksak");
            var eq2 = CreateEquipment(-2, "Kaciga");

            tour.SetRequiredEquipment(new[] { eq1, eq2 });

            // Act – prosledimo praznu listu
            tour.SetRequiredEquipment(Array.Empty<Equipment>());

            // Assert – tura više nema opremu
            tour.Equipment.ShouldBeEmpty();
        }

        [Fact]
        public void SetRequiredEquipment_throws_when_tour_is_archived()
        {
            // Arrange
            var tour = CreateDraftTour();
            tour.SetStatus(TourStatus.Published);
            tour.Archive(DateTime.UtcNow);

            var eq1 = CreateEquipment(-1, "Ruksak");

            // Act + Assert
            Should.Throw<InvalidOperationException>(() =>
            {
                tour.SetRequiredEquipment(new[] { eq1 });
            }).Message.ShouldBe("Nije moguće menjati opremu za arhiviranu turu.");
        }
    }
