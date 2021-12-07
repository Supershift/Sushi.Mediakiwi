    --Environment;
    IF:select count(*) from wim_Environments;
	THEN: INSERT INTO wim_Environments(Environment_Name , Environment_RepositoryFolder,Environment_Smtp ,Environment_Version, Environment_Created) VALUES ('Mediakiwi', '/assets/' , '127.0.0.1' , 0.0 , getdate ());

    --Role;
    IF:select count(*) from wim_Roles;
    THEN: INSERT INTO wim_Roles(Role_Name, Role_GUID, Role_Description, Role_CanSeePage, Role_CanSeeList, Role_CanSeeAdmin, Role_Page_CanChange, Role_Page_CanCreate, Role_Page_CanPublish, Role_Page_CanDelete, Role_All_Sites, Role_All_Lists, Role_CanSeeGallery, Role_CanSeeFolder, Role_List_IsAccessList,Role_List_IsAccessSite,Role_List_IsAccessFolder,Role_List_IsAccessGallery,Role_All_Folders,Role_All_Galleries, Role_List_CanCreate, Role_List_CanChange) VALUES('Super admin', '175C55BE-44B1-464B-81A4-A514AA285906', 'Initial setup',1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1);

    --User;
    IF:select count(*) from wim_Users;
    THEN: INSERT INTO wim_Users(User_Role_Key, User_GUID, User_Name, User_Email, User_Displayname, User_Password, User_IsActive,User_Language, User_DetailView, User_Reset) VALUES(1, 'F9C91400-A7B2-4199-810A-50E9E4CA8926', 'administrator', 'admin@supershift.nl', 'administrator', 'fYyuCbLOCdk=', 1,1, 0, '2BC9A748-975A-4566-A930-A5863473B4FD');
    
    --Sites;
    IF:select count(*) from wim_Sites;
    THEN: INSERT INTO wim_Sites(Site_Displayname, site_GUID, Site_Type, Site_Created, Site_IsActive, Site_HasPages, Site_HasLists, Site_InheritStructure, Site_IsRemote) VALUES('Administration', '73347829-FC58-4194-A8E9-B359C66577AC', 1, getdate(), 1, 1, 1, 0, 0);
    THEN: INSERT INTO wim_Sites(Site_Displayname, site_GUID, Site_Created, Site_IsActive, Site_HasPages, Site_HasLists, Site_InheritStructure, Site_IsRemote) VALUES('default application', 'DB0EAB0D-BE1C-494F-9146-66A9DE52DF91', getdate(), 1, 1, 1, 0, 0);

    --Folder;
    IF:select count(*) from wim_Folders;
    THEN: INSERT INTO wim_Folders (Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) VALUES (3, '9CC52C82-E977-4551-A32C-1D07E70912EF', 1, '/', '/', getdate(), 1);
    THEN: insert into wim_Folders (Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Folder_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (3, 'F330FED3-235A-4CD1-BC86-6A59309666EB', 1, 1, 'Templates', '/Templates/', getdate(), 1);
    THEN: insert into wim_Folders (Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Folder_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (3, 'BC99C244-52DF-467B-94F9-E2F7DCA42134', 1, 1, 'User management', '/User management/', getdate(), 1);
    THEN: insert into wim_Folders (Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Folder_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (3, 'B0FA55FA-DE5E-46C5-9EB1-812C77424FA3', 1, 1, 'Configuration', '/Configuration/', getdate(), 1);
    THEN: insert into wim_Folders (Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Folder_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (3, '7E3C5974-FB5F-475A-8AC1-CDAB7CC694E0', 1, 1, 'Reporting', '/Reporting/', getdate(), 1);
    THEN: insert into wim_Folders(Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (1, 'E93B4D01-AE02-49BA-9691-5B657BC1723D', 2, '/', '/', getdate(), 1);
    THEN: insert into wim_Folders(Folder_Type, Folder_GUID, Folder_Site_Key, Folder_Name, Folder_CompletePath, Folder_Created, Folder_IsVisible) values (2, '7112277E-96F1-4700-8956-07567B102086', 2, '/', '/', getdate(), 1);

	IF>: select count(*) from wim_Folders where not Folder_Folder_Key is null and Folder_GUID = '0CBA2CA4-25FD-48B4-A3BB-B760C345CF8B';
	THEN: Update wim_Folders set Folder_Site_Key = (select site_Key from wim_Sites where site_Guid = 'DB0EAB0D-BE1C-494F-9146-66A9DE52DF91'), Folder_Folder_Key = (select folder_Key from wim_Folders where Folder_GUID = '7112277E-96F1-4700-8956-07567B102086') where Folder_GUID = '0CBA2CA4-25FD-48B4-A3BB-B760C345CF8B';

    --Gallery;
    IF:select count(*) from wim_Galleries;
    THEN: insert into wim_Galleries(Gallery_Name, Gallery_GUID, Gallery_CompletePath, Gallery_Type, Gallery_IsFixed, Gallery_Created, Gallery_IsFolder, Gallery_IsActive) values('/', '97595627-E916-4117-9E68-CF957B2F36E6','/',0, 1, getdate(), 0, 1);
    
    --Countries;
    IF:select count(*) from wim_Countries;
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7DDE3F1A-3900-4597-B3B0-CB967DC43BD4','Afghanistan','Afghanistan', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D5FFCB53-7A85-4DB8-A51D-E9C6373D9C3E','Albania','Albani�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('47D70827-CE04-4BEA-9A41-418A6BFD614B','Algeria','Algerije', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F618BB9B-AC76-4D6C-A275-B58F4BE4ED0F','American Samoa', 'Amerikaans-Samoa', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('181AB5B5-7777-499F-8BD0-293F98F005A0','Andorra','Andorra', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E5F709E8-D45B-4733-8A20-65C5F5C4FE6E','Angola','Angola', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F3AABDBC-E23F-4B85-B979-AF7E800BB1E0','Anguilla','Anguilla', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C6265D06-BEC5-49AD-ADC1-2DDDD309F129','Antarctica','Antarctica', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9EB390A2-5549-49C7-99E4-ECDA84D119A7','Antigua and Barbuda','Antigua en Barbuda', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7BF608A0-81EF-4617-B77D-18F1CCB50258','Argentina','Argentini�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('653873C1-339B-4457-A270-44F383EFA1DC','Armenia','Armeni�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('084CD41D-9C9D-4FC9-9640-64CE0685EF62','Aruba','Aruba', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7A8F42B6-8AE8-4AB6-809E-BBBEA510F538','Ashmore and Cartier Islands','Ashmore en Cartier Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2745CD0C-AD00-4E81-8B22-79C48B225D64','Australia','Australi�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('EA80BBAD-65A2-4787-9FD7-D45C41B33713','Austria','Oostenrijk', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('845A1322-6432-4DB9-9EC3-DC8E6787AAB3','Azerbaijan','Azerbeidjaan', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0572E205-7809-4A0E-AD32-F1657F9D6A2B','Bahamas, The', 'Bahama''s, De', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('68EE426F-226A-419B-8502-C17B0C5204D3','Bahrain','Bahrein', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('67AB8143-AE62-42CA-A9EA-4CCDA7D91136','Baker Island', 'Baker eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D8474CEB-7131-4E67-9847-B695AB7A8209','Bangladesh','Bangladesh', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B73699A1-530C-4D8B-A297-B8318714ED9A','Barbados','Barbados', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('04254F51-4DC9-464F-8C1D-5869005765BA','Bassas da India','Bassas da India', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5915314D-3E5B-4CE0-9791-1C3F7DD98F8C','Belarus', 'Wit Rusland', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E49D19DC-BCD9-40AB-B19C-1851996353DC','Belgium','Belgi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E3A0D2B4-5E7C-4415-9450-1C64EF14E5D7','Belize','Belize', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9A649BB7-C1FB-4AC4-AE8D-02B3D1EB8AB2','Benin','Benin', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3E1662BF-DF9C-4139-8CA0-0B65AE66B93D','Bermuda','Bermuda', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('36E773EC-A037-477E-86A9-47981BD1DDA9','Bhutan','Bhutan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BC528468-9E9D-45C6-B12C-20C8F5C276C6','Bolivia','Bolivia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BBA8DEBB-6AF6-4426-84FF-1C2D18267326','Bosnia and Herzegovina','Bosni�-Herzegovina', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9D94A33E-47D6-454F-83D0-BA0718D36D02','Botswana','Botswana', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('90084534-78EA-4B74-B7D8-EC5101740B49','Bouvet Island','Bouvet Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('92FCAC4C-958A-4FF7-BCFB-D8AFFE99E333','Brazil','Brazili�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('828668BD-B7E4-4187-8630-DB0BB0C972A3','British Virgin Islands','Britse Maagden Eilanden', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('74F4285B-5E50-48C7-BEFB-F9497B26A808','Brunei','Brunei', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1370E1E1-E7F1-4136-ABDF-CDF2D5BD550B','Bulgaria','Bulgarije', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E764CA42-A1CC-4806-8EF8-FFA21EB7E379','Burkina Faso','Burkina Faso', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('46561E27-FAD0-4545-8D0F-A62E03770D3D','Burma','Burma', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('85775F06-66A8-4016-A6D4-5618EAEF3832','Burundi','Burundi', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('45BC3B08-E898-4ECA-9DC0-A0BD0464BE32','Cambodia','Cambodja', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F25804A4-D7AA-4770-B8A6-920B02332943','Cameroon','Kameroen', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7A2EF746-8E2B-4AA1-814E-09000A4DB4A2','Canada','Canada', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7F925761-E098-435F-BC67-804C698B014E','Cape Verde','Kaap Verdie', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3B1FCD13-7B52-4D7D-9F8E-6C7B324BACAB','Cayman Islands','Kaaiman-eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('655098B1-FC92-481E-A85D-DC489B383689','Central African Republic', 'Centraal Afrikaanse Republiek', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3DA8B152-741F-422A-A71D-03E25F2114B8','Chad','Gstaad', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('27EAF914-24A7-4815-8A0A-26845B5A54EB','Chile','Chili', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D194142E-1F5A-4337-8FCF-9E4CDE186DD2','China','China', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C27C975D-DD62-40C7-BD43-8E49F64A0C49','Christmas Island', 'Kerstmis Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('935BE1B2-B926-4AE1-BE7E-2ACC082C26A7','Clipperton Island','Clipperton, Eiland van', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A5ED8AFD-3DF2-4118-99E2-CD0C6C0225AE','Cocos (Keeling) Islands', 'Cocoseilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7DE5DB59-7AF4-4413-9124-AD8C9EFF5A68','Colombia','Colombia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6E0D4BEC-E70C-4539-96AB-873FFE3EC15E','Comoros','Comoros', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B8497CBF-E6ED-410F-BDF7-FA9F0885FB33','Congo, Democratic Republic of the','Za�re (Democr. Rep. Kongo)', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0149EE6E-1F52-44F7-BE1B-7736ECEDD2CF','Congo, Republic of the','Kongo', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BC8E76DF-025C-470C-87C2-1AEBF05E224B','Cook Islands', 'Cook eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9D4B372D-D8CB-4D2F-A3EB-A9FAD599FBF6','Coral Sea Islands', 'Koraal zee eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6DCE4C0B-0A40-4BFB-B065-0C1A9155BFFE','Costa Rica','Costa Rica', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CB22FCD3-0453-49F9-AA96-A5BC881026DB','Cote d''Ivoire','Ivoorkust', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('8AAC5EBC-7F9E-403B-9D5E-23762859C265','Croatia','Kroati�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('41962439-F680-420B-9007-629C2C42B400','Cuba','Cuba', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('24CE8E34-22AB-447B-9AE6-6E85FE2382FB','Cyprus','Cyprus', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A23E9082-735F-4E9D-97BD-90F375E7408C','Czech Republic', 'Tsjechi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3192E7EA-BF9D-4B04-8CA4-7EF55B002765','Denmark','Denemarken', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('91DF004A-1488-4FCE-83F4-EF5812A37F27','Djibouti','Djibouti', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7404B2EE-4369-488A-988F-B3A0F91D5595','Dominica','Dominica', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('75D68B49-E059-4A8E-A09C-D934A141237E','Dominican Republic','Dominicaanse Republiek', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9B80A4B9-5FE7-4205-877E-290AFB7FFFCE','East Timor', 'Oost Timor', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1467EA8D-E658-40C5-9676-4C7DFF8AB992','Ecuador','Ecuador', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2484E5DA-CD07-412E-BD07-6B58462904B9','Egypt','Egypte', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('EA7B5274-7317-4206-B434-8036BF29CBB6','El Salvador','El Salvador', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D233778F-48C3-48B6-A8FD-4C28E9C491D4','Equatorial Guinea', 'Equatoriaal Guinea', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('818D587C-568A-4ABC-BAF1-A6B4401F2AA7','Eritrea','Eritrea', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B2748474-16CA-4D70-9C7F-0F21681E9955','Estonia', 'Estland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6A2F28FA-F3EC-4F67-8488-BE04BAA3A072','Ethiopia','Ethiopi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F09C19A9-F5C9-41EE-A5BE-8D887EBDF810','Europa Island', 'Europa-eiland', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('89576455-F3A6-458E-B95F-9BD37C5357FB','Falkland Islands (Islas Malvinas)', 'Falkland eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('392F14BB-F94C-48FE-BE49-28D70A6F7189','Faroe Islands', 'Faroe eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E04D8672-CD14-4186-A44C-82464868EA17','Fiji', 'Fiji', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('177991DA-8C7C-4C56-97B6-D9F6B17514C8','Finland','Finland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E5B67165-A266-4738-B09A-14FA523EC2BA','France','Frankrijk', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4B13BB7F-3EB5-4B25-9517-A2419E95F322','French Guiana', 'Frans Guyana', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7CAEEA8D-F648-43B0-86A5-6F10E2417881','French Polynesia', 'Frans Polynesi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('85A10D97-1EF6-4A8F-92F6-478FEA3ED98F','Gabon', 'Gabon', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C0D5B8B3-ECBE-4363-9F0C-1713598FFCB0','Gambia, The','Gambia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('ACD021D8-858E-47B6-AEF9-1F4570CF382B','Gaza Strip','Gaza Strook', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7AD127AE-8CC9-4F28-BC98-917CA82D9376','Georgia','Georgi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('DC126F80-2DEF-4A9C-9608-ED90275DDD20','Germany','Duitsland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('481F7E50-9D13-4828-8DC1-E99EA78C652C','Ghana','Ghana', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5F940BB6-CA54-4031-B58A-99CEBCE0CE5D','Gibraltar','Gibraltar', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0C96397D-9962-4F20-8830-A74DEF16E0C1','Glorioso Islands', 'Glorioso eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D3F1A24B-8DAB-4DB7-998C-9599B362EB37','Greece','Griekenland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6C0A79F4-D097-4C4B-BC34-2EB5308AFC3A','Greenland', 'Groenland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B8CE26BA-E935-4D29-B1BA-F0D0182A2A4D','Grenada', 'Grenada', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E1E2E1EA-5FC5-44C4-8EE8-D34ADCC4E543','Guadeloupe', 'Guadeloupe', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('329B1E0C-333C-4D6D-9F52-D9A5CDAD8CE4','Guam','Guam', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C707B242-16F6-4895-A2DD-99803A84715B','Guatemala','Guatemala', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('AFE43DCB-C729-4028-8109-279B5F162A21','Guernsey','Guernsey', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E62554F2-1F24-4E31-BA97-F5CA2776B920','Guinea','Guinee', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1A90D2D4-9E8F-406D-965B-FC68E1B669EB','Guinea-Bissau', 'Guinea-Bissau', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6F088CEC-2B0B-4F95-8B83-8874C8C1EA25','Guyana','Guyana', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('ACB10842-7563-4FC0-A9AF-E9EE13A31433','Haiti','Ha�ti', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E0D8AE29-C12B-4CE4-86FD-DEC2BE87FB80','Heard Island and McDonald Islands', 'Heard en McDonaldeilanden, De', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4A99A549-31D3-4ACC-B27B-7CB9F15B1476','Holy See (Vatican City)', 'Vaticaan stad', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('010B5E34-D061-4DF9-953B-59F60DAB2144','Honduras','Honduras', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5860B969-4A06-407D-A495-9D9FE71BB5D5','Hong Kong','Hong Kong', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('87557F59-436C-4D9F-8759-55110EA28190','Howland Island','Howland Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0A80115A-7E42-4D3E-BA12-0A684A03F8F7','Hungary','Hongarije', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D3F62BDB-F205-44BA-A622-59A20B9583C9','Iceland','IJsland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4C6E44F9-E70B-403D-9D17-0220F172380A','India','India', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('24862B32-5792-4684-AB73-207BE79E9040','Indonesia','Indonesi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('406A9D38-F5C8-4CE2-83D4-25640B3159EB','Iran','Iran', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('986A00D1-5608-4652-930E-F12F32065420','Iraq','Irak', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('99BEB1B9-4A16-441C-B9BB-0B124724F4C8','Ireland','Ierland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('ECA15D64-DBB8-439B-9FBF-0B28F2A70BC0','Isle of Man','Man Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CC075053-0C85-475D-B3B8-3BB863250CCB','Israel','Isra�l', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FCB30221-B506-4A24-BF0F-34E884FA2213','Italy','Itali�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('77785234-C8EB-48CB-90BD-D41222F606FE','Jamaica','Jamaica', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('DE0FA844-BE45-4284-9437-758DFDD4BD5B','Jan Mayen','Jan Mayen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7F1BCAA5-3909-4FAA-8175-C91AAE187E83','Japan','Japan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0CBFF522-7D7F-45BE-B7F7-66056F7AB94B','Jarvis Island', 'Jarvis Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('60024F1A-5F8C-4521-8D71-239D8D45B198','Jersey','Jersey', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F29E425F-7BFD-4BC3-B776-C28120D36472','Jordan','Jordani�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A33EF724-CBDE-4A98-B149-0C3B07043F8B','Kazakhstan','Kazachstan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BB40B210-64D7-4130-9BEF-B0554E52795E','Kenya','Kenia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B69A6807-A6D9-4F83-B9E5-18558FBC9750','Kiribati','Kiribati', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BE61A895-99C3-4777-91DD-C60D82F849FC','Korea, North','Korea, Noord', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('720E1626-D314-4F8E-B33D-7D99F611B442','Korea, South','Korea, Zuid', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9287F6A0-E978-46FF-95A4-804281CD4779','Kuwait','Koeweit', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F4696626-0B18-45FA-9E81-D9F8D8E811FC','Kyrgyzstan','Kyrgysztan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FCD4A967-DAE4-480E-A679-56B04215793B','Laos','Laos', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('01C5AA0C-A100-474A-86E1-A11BBE2117AB','Latvia','Letland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CB94D1BB-F500-467B-BA01-8D9C851F0171','Lebanon','Libanon', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0FC3208F-1D21-4EFC-B5AB-8915DF98C4C5','Lesotho','Lesotho', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('AEDFB27D-5E2C-469F-A328-12DC0212D93C','Liberia','Liberia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E0FE82AB-8839-418A-A48E-CDC14E3EF486','Libya','Libi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('14FB37F8-944A-4CDD-AAD8-F567BBD471F1','Liechtenstein','Liechtenstein', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('122CAD8F-7B1B-4EE2-9556-6497CFB525BC','Lithuania','Litouwen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3742295F-DE77-40F1-B679-28C554DB8852','Luxembourg','Luxemburg', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C28F2CD8-E683-41C6-B87F-DC0B965736AD','Macau','Macau', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('72DE2CA8-EE5D-425C-A0EC-65920EE6529A','Macedonia','Macedoni�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('6A8FFAD3-2AF9-40CD-A16D-766BA39E4F2F','Madagascar','Madagascar', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C9DE1C4B-4706-4B8A-A864-FA3405B93809','Malawi','Malawi', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F412CB26-9D87-4131-A1B2-622DCF721B7F','Malaysia','Maleisi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('22135C46-B0CF-4ACC-8FC2-506DD5547A30','Maldives','Maladiven', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('73603881-1943-473E-B0A7-D348A9295577','Mali','Mali', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FA8D5815-C066-42BF-9342-D09D3204F044','Malta','Malta', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E874A7FD-211A-4D46-A4F0-C791551A3172','Marshall Islands','Marshall Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('262427EE-ED6B-4413-9F69-E2F951A1C830','Martinique','Martinique', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1C618A33-F2EA-4F9B-9866-0BBB95A6841A','Mauritania','Mauritani�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D99ACB43-A89B-4806-8CB4-0E12BE216435','Mauritius','Mauritius', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0D45640F-711D-4F5D-AB8A-902FFFE7AB16','Mexico','Mexico', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A63A16C1-6C12-4E3D-81AD-9EED7BA1CC17','Micronesia, Federated States of', 'Micronesia, De Federale Staten van', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('AAD7DDE3-5282-4A06-B3C1-B537BC984E38','Midway Islands','Midway Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('23139945-409A-4F34-8045-1273302BC0C2','Moldova','Moldavi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('EC6C611A-B6D8-4ED0-B4AC-88CA93BDF65B','Monaco','Monaco', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7E5BA6B9-CC2F-4302-9B70-BBDE9E956913','Mongolia','Mongoli�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('996DD671-9EA5-4931-A54A-AE07112507D7','Montserrat','Montserrat', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('8436C445-F3A2-4F42-939A-44A2E50667D4','Morocco','Marokko', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('095757A6-384A-4B03-BA96-D82A0E5135D4','Mozambique','Mozambique ', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('492495F1-9177-47B3-BE48-DD9A85AB706C','Namibia','Namibi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C1AD0565-6E33-4278-847F-CDC767C8DA29','Nauru','Nauru', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2F5F1149-D258-480E-990F-29A547644236','Navassa Island', 'Navassa Eland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A588362F-E5BC-431E-9F82-179D63C6EE66','Nepal','Nepal', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('8E2B000F-21CD-4A09-870D-8BEAFF8D498B','Netherlands','Nederland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('404ADD4B-0279-48A2-8255-CABB6B548261','Netherlands Antilles','Nederlandse Antillen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('19C595E8-645B-480E-8BFC-13D992457191','New Caledonia','Nieuw Caledonia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2B0D9AA9-BCA8-4CAF-9C8D-2315256B0113','New Zealand','Nieuw Zeeland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('DD1666EE-822A-44C8-9AEA-7A63823BB8E0','Nicaragua','Nicaragua', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('18527880-F6CF-4ACE-A59A-C5DEECC760CD','Niger','Niger', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('8431F543-7A5E-46AC-9B48-DB3B5849946E','Nigeria','Nigeria', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1A2DA963-F203-42A7-9E90-EE41E3670ECB','Niue','Niue', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('AFBD7D70-AA0C-4A47-9A3E-0E1C3260C719','Norfolk Island','Norfolk Eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('86865C65-280D-478A-912C-16EF0BA9DE98','Northern Mariana Islands','Noordelijke Marianen, De', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('94D0AC2D-6B12-4FAA-8B31-E237119ADFB8','Norway','Noorwegen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A113FC39-66A2-4B6F-AE93-4E456CBA3695','Oman', 'Oman', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A9E128EF-72A3-4F17-8402-BB3F93A5AC91','Pakistan','Pakistan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D3098CB0-401E-4709-89AB-A4C1AE7B5319','Palau','Palau', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BBD9C777-545D-4AB3-B28E-57D622647F23','Panama','Panama', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('294A9481-FABF-459C-8F99-7E9BEA380B25','Papua New Guinea', 'Papua Nieuw Guinea', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('96AC59C7-037C-4D6E-976E-94A17522A3EC','Paracel Islands', 'Paracel Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9F68448E-B103-41CE-9D0D-C9AEDF4FDDAB','Paraguay','Paraguay', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A3F5F1CD-0A80-4221-8C2D-05C45D0B4A56','Peru','Peru', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('890EFA74-8451-4F8A-AB23-91B4317E6642','Philippines','Filippijnen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9AE7D879-4F2A-466A-BCCC-C769F36A0EE7','Pitcairn Islands','Pitcairn Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('64FE8ED8-CCD9-415B-ABC2-F02B3DAF2FFF','Poland','Polen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FC1F1156-322C-427F-A60D-0137B596FAAC','Portugal','Portugal', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3954E64D-CB5F-4DB6-9B62-52A99A7657CB','Puerto Rico','Puerto Rico', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7962E69E-1B0C-42FD-A905-518179AF99CE','Qatar','Qatar', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F9AE3A2C-D65C-45E0-BBCD-528706854CA3','Reunion', 'R�union', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2A4AB3DD-3A58-4426-9FB0-2123D2CEBA14','Romania','Roemenie', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D087110E-6D17-4940-8EB6-2822375D91D4','Russia','Rusland', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('967E047D-CC6B-49E1-87ED-F6B71BA69266','Rwanda','Rwanda', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('8894882F-15FD-40AC-960E-91F96FDF73D0','Saint Helena', 'Sint-Helena', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5C90D88A-E4DE-4311-9E42-616143618ABC','Saint Kitts and Nevis', 'Saint Kitts en Nevis', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A9DD80FC-3EF5-4C1B-B636-6A0BD9780B79','Saint Lucia', 'Saint Lucia', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('852D827A-F38C-46D6-A93E-9B1CC2972465','Saint Pierre and Miquelon', 'Saint-Pierre en Miquelon', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('65B9119F-CC24-489F-BA3D-EF4B07D43179','Saint Vincent and the Grenadines', 'Saint Vincent en de Grenadines', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('13EAD65F-68AD-4869-985F-8EECF99D1379','Samoa', 'Samoa', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('94207C8F-C04E-4ADB-B842-0F60351E730C','San Marino', 'San Marino', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CA10FF1F-E54B-461E-B478-33D2B8E3605C','Sao Tome and Principe', 'Sao Tom� en Principe', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('012C8FEB-BF96-4466-84F4-FE949921F424','Saudi Arabia','Saudi-Arabi�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A47B65A1-FD21-4772-AFD6-AB79B79D6258','Senegal','Senegal', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A4BC1314-1414-48A8-B82F-F469963AF016','Serbia and Montenegro', 'Servi� en Montenegro', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BFD46EDE-54CC-4258-8E1C-493C8F11BFCC','Seychelles','Seychellen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7E8E7701-6955-4336-991A-90377F88829A','Sierra Leone','Sierra-Leone', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('0B94FA2D-9EFC-4F6C-8863-C160702547D7','Singapore','Singapore', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FB58361D-17F3-4EC8-A110-3F49B9C1C69C','Slovakia','Slowakije', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C8507FDB-67B1-47DC-AF13-D8644D06CC01','Slovenia','Sloveni�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('28BA0E79-4127-4B6B-A62B-4A2AF272BCB9','Solomon Islands', 'Solomon Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C7BB4429-0251-4307-9AA9-DFF4E53189E6','Somalia','Somali�', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('9E1B95F6-22A2-47D3-8E8B-E95BAF2B3438','South Africa','Zuid Afrika', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('53DBA583-0F16-4890-B99E-FDB3A83C023B','South Georgia and the South Sandwich Islands', 'South Sandwich en de South Georgia eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A8DF3CA5-8CA9-4AF3-B9DB-27ACDF6B0CF8','Spain','Spanje', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FB6E749F-FB51-4F04-B52B-B1A5EC101B56','Spratly Islands', 'Spratly Eilanden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('20E60B9C-FCAB-49B8-A281-FCA6C3DF3D88','Sri Lanka','Sri-Lanka', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('7FFE8E3B-E1A2-45F9-B933-A4F45E309068','Sudan','Soedan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('1781FA2D-484A-4826-9D31-BF8BB63E63FE','Suriname','Suriname', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A2E16B97-ECB5-4089-9902-3C47F8C273FE','Svalbard', 'Spitsbergen', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('729848FF-C7D6-4F4A-9DC5-208820BA130F','Swaziland', 'Swaziland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CD819E7E-55AE-4B09-854C-C619E8AA49D0','Sweden','Zweden', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4738F155-A549-4806-A6FE-7E57E5E5153D','Switzerland','Zwitserland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('FB3840F9-692C-4D64-9677-2F474F9D088C','Syria','Syri�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F9A794E4-DECE-485E-A4C8-F9057AD24629','Taiwan', 'Taiwan', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('29282062-C20C-4DFA-96E0-D9FA6168BA7B','Tajikistan','Tajikistan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5119DDB9-3825-4455-B85F-D0C0082E3EC2','Tanzania','Tanzania', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E1085873-FEFE-49FE-B58A-65226D74579F','Thailand','Thailand', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('665011E7-B9FB-4DAC-935D-59E59321CFE6','Togo','Togo', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('5891A749-95C2-428D-B5C2-A8AEF758B525','Tokelau','Tokelau', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A65B3AEB-65C1-4176-9536-F96F6B16A8F7','Tonga','Tonga', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('497B8CDF-9B19-4180-8489-53CF44A432EC','Trinidad and Tobago','Trinidad en Tobago', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('CAAE6C58-443A-493C-A0FD-8813D1A427B6','Tromelin Island', 'Tromelin eiland', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('13617303-979C-404E-AB63-DDA603F0FC61','Tunisia','Tunesi�', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2AA1668A-6987-4811-BD4D-F2A97E131D91','Turkey','Turkije', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('676AF278-C203-4212-88D8-A8B4525D7542','Turkmenistan','Turkmenistan', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('96AF2A6E-5DA7-4DCD-9B9C-917D766399FE','Turks and Caicos Islands', 'Turks and Caicos Islands', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E7537D08-BBB7-4EF2-87FE-C26240566E96','Tuvalu', 'Tuvalu', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B8460A69-0C21-405A-939B-909B04040C17','Uganda','Uganda', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C9BDF17F-3AA2-4DEE-ACF2-DE330B49EE24','Ukraine','Oekra�ne', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('54B03A66-76F0-44DE-B060-51C525B52E2C','United Arab Emirates', 'Verenigde Arabische Emiraten', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('B4F5D68B-92FF-4B91-9D86-ABA798C0044C','United Kingdom','Groot Brittanie', 1); 
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('BA17E0BB-54E7-47F4-9E97-10052F87EDAB','United States','Verenigde Staten van Amerika', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('F70F6536-DED1-4CEE-A758-E3D80E5C894E','Uruguay','Uruquay', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('C908765D-9513-417B-B904-56ED70EEBED0','Uzbekistan','Uzbekistan', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('635C08D0-CA7F-4396-A638-2C17EE047F0B','Vanuatu', 'Vanuatu', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('D923BCCB-6643-4110-A66E-B27B08150A19','Venezuela','Venezuela', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('56DB6B00-AA55-46D4-A643-5A452FD979C4','Vietnam','Vietnam', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4A11DE5D-02B6-4B4A-81B0-190AC462A18B','Virgin Islands', 'Maagden Eilanden', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2538523B-D4B4-4932-9C67-06DA9AC82F70','Wake Island', 'Wake-eiland', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('3852C1F5-602E-44E1-91A8-1C5BE5164C29','Wallis and Futuna', 'Wallis en Futuna', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('2FF5DD00-39D8-4066-AE83-2E996607C24B','West Bank', 'West Bank', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('90CF42E0-6A32-48F7-8BDD-C2E3FFCC703F','Western Sahara', 'West-Sahara', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('A2553AD7-BA57-4B2B-8DFC-14D8CBA640FE','Yemen', 'Jemen', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('E699F600-259E-4BF0-9250-4D09FF3C02FC','Zambia', 'Zambia', 1);
    THEN: INSERT INTO wim_Countries(Country_Guid, Country_Name_EN, Country_Name_NL, Country_IsActive) values('4CA54F7A-7D27-4863-95B3-6F4254863982','Zimbabwe','Zimbabwe', 1);

    -- COMPONENT LISTS;
    -- User;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '9870FA8F-D9B5-47ED-89AE-CF01B6D00591';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(3, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.User', 2, 'Users', 'User', null, 1, '9870FA8F-D9B5-47ED-89AE-CF01B6D00591', 1, 1, 0);
    
    -- Role;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '770DBAEA-19A9-48E0-B3D0-B2E8630C4F99';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(3, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Role', 2, 'Roles', 'Role', null, 2, '770DBAEA-19A9-48E0-B3D0-B2E8630C4F99', 1, 1, 0);

    -- Site;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '18B297DC-7E01-404D-BC45-BB3EA3EB344E';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Site', 2, 'Channels', 'Channel', null ,3, '18B297DC-7E01-404D-BC45-BB3EA3EB344E', 1, 1, 0);

    -- Image;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'f6252c60-cff3-4c8b-922e-f1d1299cca43';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Image', 2, 'Images', 'Image', null, 4, 'f6252c60-cff3-4c8b-922e-f1d1299cca43', 0, 1, 0);

    -- Link;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '161B60C4-75A6-4B54-A710-1E1691F738B4';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Link', 2, 'Links', 'Link', null, 5, '161B60C4-75A6-4B54-A710-1E1691F738B4', 0, 1, 0);
	
	-- Code Plus
	IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '793C9C8B-8713-4556-AB68-D284F81E187F';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.CodePlus', 2, 'Codelplus', 'Codelplus', null, null, '793C9C8B-8713-4556-AB68-D284F81E187F', 0, 1, 0);
    
	-- Document;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'B1F5989B-2237-4B45-AA7B-917E3979E95E';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Document', 2, 'Documents', 'Document', null, 6, 'B1F5989B-2237-4B45-AA7B-917E3979E95E', 0, 1, 0);

    -- ComponentTemplate;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'A0D67C61-0FEE-4F96-86E7-CDD53D89DC43';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentTemplate', 2, 'Component templates', 'Template', null, 8, 'A0D67C61-0FEE-4F96-86E7-CDD53D89DC43', 1, 1, 0);

    -- ComponentList;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '814FBDCD-7823-4EF1-8DD5-A347D8FDC090';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentList', 2, 'Component lists', 'Template', null, 9, '814FBDCD-7823-4EF1-8DD5-A347D8FDC090', 1, 1, 0);

    -- PageTemplate;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '5017D373-C45D-4C8F-8276-BE881FA08BD6';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PageTemplate', 2, 'Page templates', 'Template', null, 10, '5017D373-C45D-4C8F-8276-BE881FA08BD6', 1, 1, 0);

	-- Installer;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '3b8fe32a-7a82-45f6-8fa5-75b52cd1499a';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.UI.InstallerList', 2, 'Installed modules', 'Module', null, null, '3b8fe32a-7a82-45f6-8fa5-75b52cd1499a', 1, 1, 0);

    -- Notification;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'B2F79749-2F07-4383-9B75-665AE4B46D71';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(5, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Notification', 2, 'Notifications', 'Notification', null, 11, 'B2F79749-2F07-4383-9B75-665AE4B46D71', 1, 1, 0);

    -- Environment;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '503f862b-16c3-486b-9558-3e006058c624';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Environment', 2, 'Environments', 'Environment', null, 13, '503f862b-16c3-486b-9558-3e006058c624', 1, 1, 0);

    -- Folder;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '97292DD5-EBDA-4318-8AAF-4C49E887CDAD';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Folder', 2, 'Folders', 'Folder', null, 21, '97292DD5-EBDA-4318-8AAF-4C49E887CDAD', 0, 1, 0);

    -- Browsing;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '0B510F56-83F6-41B8-8C2A-15066024BCD6';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing', 2, 'Browsing', 'Browsing', null, 22, '0B510F56-83F6-41B8-8C2A-15066024BCD6', 0, 1, 0);

	-- Export file;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '775b3f09-1c2f-4684-b7bc-da3fa358dc99';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.ExportFile', 2, 'Export file', 'Export file', null, null, '775b3f09-1c2f-4684-b7bc-da3fa358dc99', 0, 1, 0);

    -- ComponentListInstance;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'D3EB0351-A7BD-460B-8349-1EEF0A2E5771';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentListInstance', 2, 'List template instance', 'Instance', null, 23, 'D3EB0351-A7BD-460B-8349-1EEF0A2E5771', 0, 1, 0);

    -- ComponentListProperties;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '1562120C-1C37-49A5-83CD-6B39EBD7F12F';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentListProperties', 2, 'List properties', 'properties', null, 24, '1562120C-1C37-49A5-83CD-6B39EBD7F12F', 0, 1, 0);

    -- PageInstance;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '4E7BCF0F-844B-4877-AB2D-3154BE01BC0F';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PageInstance', 2, 'Page properties', 'Page', null, 25, '4E7BCF0F-844B-4877-AB2D-3154BE01BC0F', 0, 1, 0);

    -- Property;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '6A7D5E6C-9DAA-4A6F-AEEA-21BD4782DA1E';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Property', 1, 'Fields', 'Fields', null, 29, '6A7D5E6C-9DAA-4A6F-AEEA-21BD4782DA1E', 0, 1, 0);

    -- PropertyOption;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '9CA13663-54E0-4701-9658-E2DDDF769F78';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PropertyOption', 1, 'Options', 'Options', null, 30, '9CA13663-54E0-4701-9658-E2DDDF769F78', 0, 1, 0);

    -- PropertySearchColumn;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'C7B2C21D-0121-4E58-A0C9-616B31C2E1B1';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PropertySearchColumn', 1, 'Columns', 'Columns', null, 31, 'C7B2C21D-0121-4E58-A0C9-616B31C2E1B1', 0, 1, 0);

    -- ComponentListData;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'ABEF9E0A-F5A6-45B7-B812-1B3C861885A0';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentListData', 2, 'Data', 'Data', null, 33, 'ABEF9E0A-F5A6-45B7-B812-1B3C861885A0', 0, 1, 0);

    -- ComponentListSettings;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '15C30414-F187-4021-B991-386653677767';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentListSettings', 2, 'Settings', 'Settings', null, 46, '15C30414-F187-4021-B991-386653677767', 0, 1, 0);

    -- Menu;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '3ab5b10f-f4ae-4f61-9db1-7f938ccc4f37';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.MenuList', 2, 'Menu', 'Menu', null, null, '3ab5b10f-f4ae-4f61-9db1-7f938ccc4f37', 1, 1, 0);

	-- Search;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '1a1fe050-219c-4f63-a697-7e2e8e790521';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.BasicSearchList', 2, 'Search', 'Search', null, null, '1a1fe050-219c-4f63-a697-7e2e8e790521', 1, 0, 0);

    -- GenericExposedListCollection;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '677C8CD4-9C2F-43A2-9ED8-EDABA06A3F97';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(2, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.GenericExposedListCollection', 1, '_Shared list collections', 'Datalist', null, null, '677C8CD4-9C2F-43A2-9ED8-EDABA06A3F97', 0, 1, 0);

    -- Note list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '30438020-85D2-4160-B7A9-E8E6A2A53E65';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.NoteList', 2, 'Note', 'Note', null, 38, '30438020-85D2-4160-B7A9-E8E6A2A53E65', 0, 1, 0);

    -- Portal;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '6B515D23-543F-4971-A0DD-E051742C6F94';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PortalList', 2, 'Portals', 'Portal', null, 40, '6B515D23-543F-4971-A0DD-E051742C6F94', 0, 1, 0);

    -- Role access list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'B502AA2C-8D85-4D2C-9470-DFC8A388AC72';
    THEN: INSERT INTO wim_ComponentLists (ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.RoleAccessList', 2, 'Lists', 'Lists', null, 42, 'B502AA2C-8D85-4D2C-9470-DFC8A388AC72', 0, 1, 0);

    -- Role access site;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '93D10F58-6A1A-493F-8ADB-E53FC7CEDE19';
    THEN: INSERT INTO wim_ComponentLists (ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.RoleAccessSite', 2, 'Sites', 'Sites', null, 43, '93D10F58-6A1A-493F-8ADB-E53FC7CEDE19', 0, 1, 0);

    -- Role access folder;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'AB58D901-7305-4584-9133-53FB92684A2C';
    THEN: INSERT INTO wim_ComponentLists (ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.RoleAccessFolder', 2, 'Folders', 'Folders', null, 44, 'AB58D901-7305-4584-9133-53FB92684A2C', 0, 1, 0);
    
    -- Role access gallery;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '5A7DB5D1-8DCE-4510-8423-47E4925B6C8B';
    THEN: INSERT INTO wim_ComponentLists (ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.RoleAccessGallery', 2, 'Galleries', 'Galleries', null, 45, '5A7DB5D1-8DCE-4510-8423-47E4925B6C8B', 0, 1, 0);
   
    -- Page list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '63c0c71c-e301-4a29-9a75-73874cb6622e';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PageList', 2, 'Pagelist', 'Pagelist', null, 28, '63c0c71c-e301-4a29-9a75-73874cb6622e', 1, 0, 0);

	-- Pagemapping list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '5dfe77be-a7c3-4494-87b2-f9ffba97ba52';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PageMappingList', 2, 'PageMapping', 'PageMapping list', null, null, '5dfe77be-a7c3-4494-87b2-f9ffba97ba52', 0, 1, 0);
	
	-- Copy page/folder list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '1FC0AA4D-A38F-4E29-AF49-996DDD31936A';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Copy', 2, 'Copy', 'Copy', null, null, '1FC0AA4D-A38F-4E29-AF49-996DDD31936A', 0, 1, 0);

	-- Copy page content list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'a1c0c71c-A38F-4E29-AF49-996DDD31936A';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.CopyContent', 2, 'Copy page content', 'Copy page content', null, null, 'a1c0c71c-A38F-4E29-AF49-996DDD31936A', 0, 1, 0);

	-- Page history list;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'b1c0c71c-A38F-4E29-AF49-996DDD31936A';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_ContainsOneChild, ComponentList_IsVisible, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.PageHistory', 2, 'Page History', 'Page History', null, null, 'b1c0c71c-A38F-4E29-AF49-996DDD31936A', 0, 1, 0);

    -- ComponentListInstance;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = 'D03439F0-73D7-4C78-B51E-50310A00F6DA';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.UI.WikiList', 2, 'Wiki', 'Wiki', null, null, 'D03439F0-73D7-4C78-B51E-50310A00F6DA', 0, 1, 0);

    -- Document Type properties;
    IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '36fc7157-d5c7-433c-8317-b601226f9bd0';
    THEN: 
	INSERT INTO wim_ComponentLists (ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(null, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.UI.DocumentType_List', 2, 'DT Definition', 'DT Definition', null, null, '36fc7157-d5c7-433c-8317-b601226f9bd0', 0, 1, 0);
	
	-- SharedFieldList
	IF: select count(*) from wim_ComponentLists where ComponentList_GUID = '937C843E-C586-4CAE-A923-7305685242C4';
    THEN: INSERT INTO wim_ComponentLists(ComponentList_Folder_Key, ComponentList_Assembly, ComponentList_ClassName, ComponentList_TargetType, ComponentList_Name, ComponentList_SingleItemName, ComponentList_Description,ComponentList_Type, ComponentList_GUID, ComponentList_IsVisible, ComponentList_ContainsOneChild, ComponentList_IsInherited) VALUES(4, 'Sushi.Mediakiwi.dll', 'Sushi.Mediakiwi.AppCentre.Data.Implementation.SharedFieldList', 2, 'Shared Fields', 'Shared Field', null, 47, '937C843E-C586-4CAE-A923-7305685242C4', 1, 1, 0);
