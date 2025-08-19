(function ($) {
    var l = abp.localization.getResource('ApiKeyManagement');

    var _apiKeyAppService = abp.apiKeyManagement.apiKeys.apiKey;

    var _editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'ApiKeyManagement/EditModal'
    });

    var _createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'ApiKeyManagement/CreateModal'
    });

    var _permissionsModal = new abp.ModalManager(
        abp.appPath + 'AbpPermissionManagement/PermissionManagementModal'
    );

    var _dataTable = null;

    abp.ui.extensions.entityActions.get('apiKeyManagement.apiKeys').addContributor(
        function(actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Edit'),
                        action: function (data) {
                            _editModal.open({
                                id: data.record.id,
                            });
                        },
                    },
                    {
                        text: l('Permissions'),
                        action: function (data) {
                            _permissionsModal.open({
                                providerName: 'AK',
                                providerKey: data.record.id,
                                providerKeyDisplayName: data.record.name
                            });
                        },
                    },
                    {
                        text: l('Delete'),
                        confirmMessage: function (data) {
                            return l(
                                'ApiKeyDeletionConfirmationMessage',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            _apiKeyAppService
                                .delete(data.record.id)
                                .then(function () {
                                    _dataTable.ajax.reload();
                                    abp.notify.success(l('SuccessfullyDeleted'));
                                });
                        },
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get('apiKeyManagement.apiKeys').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get('apiKeyManagement.apiKeys').actions.toArray()
                        }
                    },
                    {
                        title: l('Name'),
                        data: 'name'
                    },
                    {
                        title: l('Description'),
                        data: 'description',
                    },
                    {
                        title: l('Key'),
                        data: 'prefix',
                    },
                    {
                        title: l('Active'),
                        data: 'isActive',
                        render: function (data, type, row) {
                            if (!row.isActive) {
                                return  'Disabled';
                            }else{
                                return  'Active';
                            }
                        }
                    },
                    {
                        title: l('ExpirationTime'),
                        data: 'expirationTime',
                        render: function (expireAt) {
                            if (!expireAt) {
                                return "";
                            }

                            var date = Date.parse(expireAt);
                            return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                        }
                    },
                    {
                        title: l('CreationTime'),
                        data: 'creationTime',
                        render: function (creationTime) {
                            if (!creationTime) {
                                return "";
                            }

                            var date = Date.parse(creationTime);
                            return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                        }
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {
        var _$wrapper = $('#ApiKeysWrapper');
        var _$table = _$wrapper.find('table');
        _dataTable = _$table.DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[6, 'desc']],
                processing: true,
                serverSide: true,
                scrollX: true,
                paging: true,
                ajax: abp.libs.datatables.createAjax(
                    _apiKeyAppService.getList
                ),
                columnDefs: abp.ui.extensions.tableColumns.get('apiKeyManagement.apiKeys').columns.toArray()
            })
        );

        _createModal.onResult(function (e, result) {
            var key = result.responseText.key;
            // Show the API key in a modal
            showApiKey(key);
            
            abp.notify.success(l('ApiKeyCreated'));
            // Reload the table to show the new API key
            _dataTable.ajax.reload();
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reload();
        });

        _$wrapper.find('button[name=CreateApiKey]').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        function showApiKey(apiKey) {
            Swal.fire({
                title: l('ApiKey'),
                icon: 'info',
                html: `
            <div style="display: flex; align-items: center; gap: 8px; justify-content: center;">
                <input id="apiKeyInput" type="text" value="${apiKey}" readonly
                       style="padding: 8px; width: 250px; border: 1px solid #ccc; border-radius: 4px; text-align: center;" />
                <button id="copyApiKeyBtn" class="swal2-confirm swal2-styled" style="padding: 6px 10px;">${l('Copy')}</button>
            </div>
        `,
                showConfirmButton: false,
                didOpen: () => {
                    const copyBtn = document.getElementById('copyApiKeyBtn');
                    copyBtn.addEventListener('click', async () => {
                        try {
                            await navigator.clipboard.writeText(apiKey);
                            Swal.fire({
                                icon: 'success',
                                title: l('Success'),
                                text: l('ApiKeyCopied'),
                                timer: 1500,
                                showConfirmButton: false
                            });
                        } catch (err) {
                            console.error(err);
                        }
                    });
                }
            });
        }

    });
})(jQuery);
