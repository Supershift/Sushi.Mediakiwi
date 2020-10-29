namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IAvailableTemplateParser
    {
        void Clear();
        bool Delete(IAvailableTemplate entity);
        void Delete(int pageTemplateID, string portal);
        void Delete(int pageTemplateID, bool isSecundary, string target);
        void Save(IAvailableTemplate entity);
        IAvailableTemplate[] SelectAll();
        IAvailableTemplate[] SelectAll(int pageTemplateID, string target = null);
        IAvailableTemplate[] SelectAll(int pageTemplateID, int pageID, bool onlyReturnFixedInCode = false);
        IAvailableTemplate SelectOne(int availableTemplateID);
        IAvailableTemplate SelectOne(int pageTemplateID, string fixedTag);
        IAvailableTemplate[] SelectAllBySlot(int slotID);
        IAvailableTemplate[] SelectAllByComponentTemplate(int templateID);
    }
}