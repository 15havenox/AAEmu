-- Example data for trade packs system
-- This file contains sample data for the specialty system

-- Insert example specialty NPCs
INSERT INTO `specialty_npcs` (`name`, `npc_id`, `specialty_bundle_id`) VALUES
('Trade Merchant 1', 1001, 1),
('Trade Merchant 2', 1002, 2),
('Trade Merchant 3', 1003, 3);

-- Insert example specialty bundle items
-- These are example trade pack items with their profit and ratio values
INSERT INTO `specialty_bundle_items` (`item_id`, `specialty_bundle_id`, `profit`, `ratio`) VALUES
-- Bundle 1 items (example trade packs)
(10001, 1, 50000, 1000),  -- Example trade pack 1
(10002, 1, 60000, 1000),  -- Example trade pack 2
(10003, 1, 70000, 1000),  -- Example trade pack 3

-- Bundle 2 items
(10004, 2, 55000, 1000),  -- Example trade pack 4
(10005, 2, 65000, 1000),  -- Example trade pack 5

-- Bundle 3 items
(10006, 3, 45000, 1000),  -- Example trade pack 6
(10007, 3, 55000, 1000);  -- Example trade pack 7

-- Insert example specialties (zone relationships)
INSERT INTO `specialties` (`row_zone_group_id`, `col_zone_group_id`, `ratio`, `profit`) VALUES
(1, 2, 1000, 0),  -- Zone 1 to Zone 2
(1, 3, 1200, 0),  -- Zone 1 to Zone 3
(2, 1, 800, 0),   -- Zone 2 to Zone 1
(2, 3, 1100, 0),  -- Zone 2 to Zone 3
(3, 1, 900, 0),   -- Zone 3 to Zone 1
(3, 2, 1300, 0);  -- Zone 3 to Zone 2