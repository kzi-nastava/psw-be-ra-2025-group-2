INSERT INTO stakeholders."PlannerDayEntries" ("Id", "TouristId", "Date", "Notes") VALUES
(-101, -21, '2026-02-13', 'Belgrade Intensive Day'),
(-102, -21, '2026-02-14', 'Nature and Water Day'),
(-103, -22, '2026-02-13', 'Novi Sad and Mountains');

INSERT INTO stakeholders."PlannerScheduleEntries" ("Id", "DayEntryId", "TourId", "Notes", "StartTime", "EndTime") VALUES
(-201, -101, -1, 'Morning walk', '2026-02-13T10:00:00Z', '2026-02-13T11:00:00Z'),
(-202, -101, -7, 'Zemun walk', '2026-02-13T11:00:00Z', '2026-02-13T13:00:00Z'),
(-203, -101, -3, 'Niš Food', '2026-02-13T14:00:00Z', '2026-02-13T16:00:00Z'),
(-204, -101, -4, 'Wine route', '2026-02-13T17:00:00Z', '2026-02-13T19:00:00Z');

INSERT INTO stakeholders."PlannerScheduleEntries" ("Id", "DayEntryId", "TourId", "Notes", "StartTime", "EndTime") VALUES
(-205, -102, -9, 'Rafting', '2026-02-14T09:00:00Z', '2026-02-14T12:00:00Z');

INSERT INTO stakeholders."PlannerScheduleEntries" ("Id", "DayEntryId", "TourId", "Notes", "StartTime", "EndTime") VALUES
(-206, -103, -9, 'City Tour', '2026-02-13T10:00:00Z', '2026-02-13T13:00:00Z'),
(-207, -103, -11, 'Street Art', '2026-02-13T14:00:00Z', '2026-02-13T16:00:00Z');
