using ModelosDB.Inventario;
using WA_StoreControl.Services;

namespace WA_StoreControl.Tests
{
    public class FakeCategoriasService : CategoriasService
    {
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeCategoriasService(
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeUpdate(Categoria Categoria)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }
}
