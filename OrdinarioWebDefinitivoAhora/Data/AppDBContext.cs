using OrdinarioWebDefinitivoAhora.Models;
using Microsoft.EntityFrameworkCore;

namespace OrdinarioWebDefinitivoAhora.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        
        public DbSet<MemoModel> Memos { get; set; }
        public DbSet<CancelacionModel> Cancelaciones { get; set; }
        public DbSet<FolioModel> Folios { get; set; }
        public DbSet<UserModel> Users { get; set; }
        // Inicializar folios para un año específico
        public void InicializarFolios(int year)
        {
            if (!Folios.Any(f => f.Year == year))
            {
                List<FolioModel> lista = new List<FolioModel>();

                for (int i = 1; i <= 500; i++)
                {
                    lista.Add(new FolioModel
                    {
                        Year = year,
                        Number = i,
                        Estado = FolioEstado.Disponible
                    });
                }

                Folios.AddRange(lista);
                SaveChanges();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MemoModel>()
                .HasOne(m => m.Folio)
                .WithOne(f => f.Memo)
                .HasForeignKey<MemoModel>(m => m.FolioId)
                .OnDelete(DeleteBehavior.Restrict);
        }





    }
}

