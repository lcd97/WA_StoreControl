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

    public class FakeSubCategoriasService : SubCategoriasService
    {
        private readonly string _validateBeforeCreateResult;
        private readonly string _validateBeforeUpdateResult;
        private readonly string _validateBeforeDeleteResult;

        public FakeSubCategoriasService(
            string validateBeforeCreateResult = "",
            string validateBeforeUpdateResult = "",
            string validateBeforeDeleteResult = "")
            : base(new ModelosDB.DBStore())
        {
            _validateBeforeCreateResult = validateBeforeCreateResult;
            _validateBeforeUpdateResult = validateBeforeUpdateResult;
            _validateBeforeDeleteResult = validateBeforeDeleteResult;
        }

        public override string ValidateBeforeCreate(SubCategoria SubCategoria)
        {
            return _validateBeforeCreateResult;
        }

        public override string ValidateBeforeUpdate(SubCategoria SubCategoria)
        {
            return _validateBeforeUpdateResult;
        }

        public override string ValidateBeforeDelete(int id)
        {
            return _validateBeforeDeleteResult;
        }
    }
}
