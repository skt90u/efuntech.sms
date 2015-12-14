using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SingleUiGridControllerGenerator : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public SchemaInfo SchemaInfo { get; set; }

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            string jsControllerName = SchemaInfo.JsControllerName;
            string apiUrl = SchemaInfo.ApiUrl;
            var modelName = SchemaInfo.ModelName;

            var schemaName = SchemaInfo.SchemaName;
            var properties = SchemaInfo.Properties;
            var searchableProperties = SchemaInfo.SearchableProperties;
            var idTypeString = SchemaInfo.IdTypeString;

            lines.Add(string.Format("(function (window, document) {{"));
            lines.Add(string.Format("    'use strict';"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    angular.module('app').controller('{0}',['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs', ", jsControllerName));
            lines.Add(string.Format("        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        //========================================"));
            lines.Add(string.Format("        // Settings"));
            lines.Add(string.Format("        //========================================"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        var crudApi = new CrudApi('{0}');", apiUrl));
            lines.Add(string.Format("        var paginationOptions = angular.copy(GlobalSettings.paginationOptions);"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        $scope.gridOptions = {{"));
            lines.Add(string.Format("            // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions"));
            lines.Add(string.Format("            paginationPageSizes: paginationOptions.pageSizes,"));
            lines.Add(string.Format("            paginationPageSize: paginationOptions.pageSize,"));
            lines.Add(string.Format("            useExternalPagination: true,"));
            lines.Add(string.Format("            useExternalSorting: true,"));
            lines.Add(string.Format("            enableColumnMenus: false,"));
            lines.Add(string.Format("            columnDefs: ["));

            foreach (var propertiey in properties)
            {
                WriteColumnDef(lines, propertiey);
            }

            lines.Add(string.Format("			{{"));
            lines.Add(string.Format("			 	name: 'Update',"));
            lines.Add(string.Format("			 	width: '50',"));
            lines.Add(string.Format("			 	displayName: '',"));
            lines.Add(string.Format("			 	cellEditableCondition: false,"));
            lines.Add(string.Format("			 	cellClass: 'grid-align-center',"));
            lines.Add(string.Format("			 	cellTemplate: '<img class=\"edit\" ng-click=\"grid.appScope.editRow(row.entity)\"/>'"));
            lines.Add(string.Format("			}},"));
            lines.Add(string.Format("			{{"));
            lines.Add(string.Format("			 	name: 'Delete',"));
            lines.Add(string.Format("			 	width: '50',"));
            lines.Add(string.Format("			 	displayName: '',"));
            lines.Add(string.Format("			 	cellEditableCondition: false,"));
            lines.Add(string.Format("			 	cellClass: 'grid-align-center',"));
            lines.Add(string.Format("			 	cellTemplate: '<img class=\"lightbulb_48\" ng-click=\"grid.appScope.deleteRow(row.entity)\"/>'"));
            lines.Add(string.Format("			}},"));

            lines.Add(string.Format("            ],"));
            lines.Add(string.Format("            data: [],"));
            lines.Add(string.Format("            onRegisterApi: function (gridApi) {{"));
            lines.Add(string.Format("                $scope.gridApi = gridApi;"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {{"));
            lines.Add(string.Format("                    paginationOptions.pageNumber = newPage;"));
            lines.Add(string.Format("                    paginationOptions.pageSize = pageSize;"));
            lines.Add(string.Format("                    $scope.search();"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }}"));
            lines.Add(string.Format("        }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format("            // Functions"));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.search = function () {{"));
            lines.Add(string.Format("                var criteria = {{"));
            lines.Add(string.Format("                    SearchText: $scope.searchText,"));
            lines.Add(string.Format("                    PageIndex: paginationOptions.pageNumber,"));
            lines.Add(string.Format("                    PageSize: paginationOptions.pageSize"));
            lines.Add(string.Format("                }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                crudApi.GetAll(criteria)"));
            lines.Add(string.Format("                .then(function (result) {{"));
            lines.Add(string.Format("                    if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                    var data = result.data;"));
            lines.Add(string.Format("                    $scope.gridOptions.totalItems = data.TotalCount;"));
            lines.Add(string.Format("                    $scope.gridOptions.data = data.Result;"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format(""));


            lines.Add(string.Format("        $scope.editRow = function (rowEntity) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {{}}); // <--- angular.copy用以避免影響GridRow"));
            lines.Add(string.Format("            var isNew = !angular.isDefined(model.Id);"));
            lines.Add(string.Format("            if (isNew) {{"));
            lines.Add(string.Format("               // TODO: 設定新增資料的初始值"));
            lines.Add(string.Format("            }}"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var schemaForm = SchemaFormFactory.create('{0}', {{isNew: isNew}});", modelName));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var title = isNew ? '新增資料' : '編輯資料';"));
            lines.Add(string.Format("            var schema = schemaForm.schema;"));
            lines.Add(string.Format("            var form = schemaForm.form;"));
            lines.Add(string.Format("            var options = {{"));
            lines.Add(string.Format("                validationMessage: {{"));
            lines.Add(string.Format("                    202: '不符合欄位格式',"));
            lines.Add(string.Format("                    302: '此為必填欄位',"));
            lines.Add(string.Format("                }}"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format("            var validateBeforeSubmit = function (modalScope, form) {{ return true; }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var modalInstance = $modal.open({{"));
            lines.Add(string.Format("                templateUrl: 'template/modal/editRow.html',"));
            lines.Add(string.Format("                controller: 'FormModalCtrl',"));
            lines.Add(string.Format("                windowClass: 'center-modal',"));
            lines.Add(string.Format("                //size: size,"));
            lines.Add(string.Format("                resolve: {{"));
            lines.Add(string.Format("                    options: function () {{"));
            lines.Add(string.Format("                        return {{"));
            lines.Add(string.Format("                            title: title,"));
            lines.Add(string.Format("                            schema: schema,"));
            lines.Add(string.Format("                            form: form,"));
            lines.Add(string.Format("                            model: model,"));
            lines.Add(string.Format("                            options: options,"));
            lines.Add(string.Format("                            validateBeforeSubmit: validateBeforeSubmit,"));
            lines.Add(string.Format("                        }};"));
            lines.Add(string.Format("                    }},"));
            lines.Add(string.Format("                }}"));
            lines.Add(string.Format("            }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            modalInstance.result.then(function (savedModel) {{"));
            lines.Add(string.Format("                if (!savedModel) return;"));
            lines.Add(string.Format("                var isNew = !angular.isDefined(savedModel.Id);"));
            lines.Add(string.Format("                if (isNew) {{"));
            lines.Add(string.Format("                    $scope.createRow(savedModel);"));
            lines.Add(string.Format("                }}"));
            lines.Add(string.Format("                else {{"));
            lines.Add(string.Format("                    $scope.updateRow(savedModel);"));
            lines.Add(string.Format("                }}"));
            lines.Add(string.Format("            }}, function () {{"));
            lines.Add(string.Format("                $log.debug('Modal dismissed at: ' + new Date());"));
            lines.Add(string.Format("            }});"));
            lines.Add(string.Format("        }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.createRow = function (model) {{"));
            lines.Add(string.Format("                crudApi.Create(model)"));
            lines.Add(string.Format("                .then(function () {{"));
            lines.Add(string.Format("                    $scope.search();"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.updateRow = function (model) {{"));
            lines.Add(string.Format("                crudApi.Update(model)"));
            lines.Add(string.Format("                .then(function () {{"));
            lines.Add(string.Format("                    $scope.search();"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.deleteRow = function (rowEntity) {{"));
            lines.Add(string.Format("                var model = rowEntity[0] || rowEntity;"));
            lines.Add(string.Format(""));

            foreach (var propertiey in properties)
            {
                WriteDeleteConfirm(lines, propertiey);
            }

            lines.Add(string.Format("                dialogs.confirm('刪除資料', message).result.then(function (btn) {{"));
            lines.Add(string.Format("                    crudApi.Delete(model)"));
            lines.Add(string.Format("                    .then(function () {{"));
            lines.Add(string.Format("                        $scope.search();"));
            lines.Add(string.Format("                    }});"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.deleteSelection = function () {{"));
            lines.Add(string.Format("                var selectedRows = $scope.gridApi.selection.getSelectedRows();"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                if (selectedRows.length == 0) {{"));
            lines.Add(string.Format("                    dialogs.error('刪除資料', '請先選擇欲刪除之資料');"));
            lines.Add(string.Format("                    return;"));
            lines.Add(string.Format("                }}"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                var message = \"確定刪除這\" + selectedRows.length + \"筆資料？\";"));
            lines.Add(string.Format("                dialogs.confirm('刪除資料', message).result.then(function (btn) {{"));
            lines.Add(string.Format("                    crudApi.Delete(selectedRows)"));
            lines.Add(string.Format("                    .then(function () {{"));
            lines.Add(string.Format("                        $scope.search();"));
            lines.Add(string.Format("                    }});"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format("            // Events & EventHandlers"));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.$on('tab.onSelect', function (event, tabName) {{"));
            lines.Add(string.Format("                if (tabName !== '{0}') return;", jsControllerName));
            lines.Add(string.Format("                $scope.search();"));
            lines.Add(string.Format("            }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format("            // Initialize"));
            lines.Add(string.Format("            //========================================"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.searchText = '';"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            $scope.search();"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    }}]);"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("}})(window, document);"));

            return lines;
        }

        private void WriteDeleteConfirm(List<string> lines, SchemaPropertyInfo propertiey)
        {
            if (propertiey.Name == "Id") return;
            if (!propertiey.IsModelProperty) return;

            lines.Add(string.Format("                var message = \"確定刪除『\" + model.{0} + \"』？\";", propertiey.Name));
        }

        private void WriteColumnDef(List<string> lines, SchemaPropertyInfo propertiey)
        {
            if (propertiey.Name == "Id") return;
            if (!propertiey.IsModelProperty) return;

            lines.Add(string.Format("            {{ name: '{0}', displayName: '{1}' }},", propertiey.Name, propertiey.DisplayName));
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetJsControllerFilePath(PortalDir);
            Utils.WriteToJavascriptFile(filePath, lines, Overwrite);
        }
    }
}
