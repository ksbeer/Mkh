using System;
using System.Threading.Tasks;
using Data.Common.Test;
using Data.Common.Test.Domain.Article;
using Data.Common.Test.Domain.Category;
using Data.Common.Test.Infrastructure;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mkh.Data.Abstractions;
using Xunit.Abstractions;

namespace Data.Adapter.PostgreSQL.Test;

public class BaseTest
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IDbContext _dbContext;
    protected readonly IArticleRepository _articleRepository;
    protected readonly ICategoryRepository _categoryRepository;

    public BaseTest(ITestOutputHelper output)
    {
        var connString = "server=localhost;uid=mkh;password=mkh;database=mkh;port=5433;commandtimeout=1024;";
        var services = new ServiceCollection();
        //��־
        services.AddLogging(builder =>
        {
            builder.AddXunit(output, new LoggingConfig
            {
                LogLevel = LogLevel.Trace
            });
        });

        //�Զ����˻���Ϣ������
        services.AddSingleton<IAccountResolver, CustomAccountResolver>();

        services
            .AddMkhDb<BlogDbContext>(options =>
            {
                //������־
                options.Log = true;
            })
            .UsePostgreSQL(connString)
            .AddRepositoriesFromAssembly(typeof(BlogDbContext).Assembly)
            .AddRepositoriesFromAssembly(this.GetType().Assembly)
            //������������
            .AddCodeFirst(options =>
            {
                options.CreateDatabase = true;
                options.UpdateColumn = true;

                options.BeforeCreateDatabase = ctx =>
                {
                    ctx.Logger.Write("BeforeCreateDatabase", "���ݿⴴ��ǰ�¼�");
                };
                options.AfterCreateDatabase = ctx =>
                {
                    ctx.Logger.Write("AfterCreateDatabase", "���ݿⴴ�����¼�");
                };
                options.BeforeCreateTable = (ctx, entityDescriptor) =>
                {
                    ctx.Logger.Write("BeforeCreateTable", "������ǰ�¼��������ƣ�" + entityDescriptor.TableName);
                };
                options.AfterCreateTable = (ctx, entityDescriptor) =>
                {
                    ctx.Logger.Write("AfterCreateTable", "���������¼��������ƣ�" + entityDescriptor.TableName);
                };
            })
            .Build();

        _serviceProvider = services.BuildServiceProvider();
        _dbContext = _serviceProvider.GetService<BlogDbContext>();
        _articleRepository = _serviceProvider.GetService<IArticleRepository>();
        _categoryRepository = _serviceProvider.GetService<ICategoryRepository>();
    }

    protected async Task ClearTable()
    {
        await _articleRepository.Execute("truncate Article;");
        await _articleRepository.Execute("truncate MyCategory;");
    }
}