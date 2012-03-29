//If you rename or remove this file, it will be re-created during package update.
tau.mashups
.addDependency("libs/jquery/jquery")
.addMashup(function (jquery, config) {
	function ProfileEditor(config) {
		this._create(config);
	}

	ProfileEditor.prototype = {
		template: null,
		placeHolder: null,
		saveBtn: null,
		_returnUrl: null,
		_pluginName: null,

		_create: function (config) {
			this.placeHolder = config.placeHolder;
			this._returnUrl = new Tp.WebServiceURL("/Admin/Plugins.aspx").toString();

			this.template = '<div>' +
                    '<form method="POST">' +
                    '<div class="task-creator-settings">' +
                    '	<div class="pad-box">' +
                    '		<p class="label">Profile Name&nbsp;<br />' +
					'		<span class="small">Once this name is saved, you can not change it.</span></p>' +
                    '		<input id="profileNameTextBox" type="text" name="Name" class="input" style="width: 275px;" value="${Name}" />' +
                    '	</div>' +
                    '<div class="save-block" style="padding:15px">' +
                    '	<a href="javascript:void(0);" id="saveButton" class="button success big" style="font-size: 13px; font-weight: bold;">Save</a>' +
                    '	<a href="' + this._returnUrl + '" style="color: #666; padding-left: 10px;">Back to plugins</a>' +
                    '</div>' +
                    '</form>' +
                    '</div>';
		},

		render: function () {
			var url = new Tp.URL(location.href);
			var profileName = url.getArgumentValue('ProfileName');
			this._pluginName = url.getArgumentValue('PluginName');

			if (profileName) {
				var requestUrl = '/api/v1/Plugins.asmx/{PluginName}/Profiles/{ProfileName}'.replace(/{PluginName}/g, this._pluginName).replace(/{ProfileName}/g, profileName);
				$.ajax({
					url: new Tp.WebServiceURL(requestUrl).url,
					dataType: 'json',
					success: $.proxy(this._renderProfile, this),
					error: $.proxy(
							function (response) {
								alert(response);
							}, this)
				});
			}
			else {
				this._renderProfile(null);
			}
		},

		_renderProfile: function (profile) {
			profile = profile || { Name: null };
			this.placeHolder.html('');

			$.tmpl(this.template, profile).appendTo(this.placeHolder);

			if (profile.Name) {
				this.placeHolder.find('#profileNameTextBox').attr('disabled', true);
			}

			this.saveBtn = this.placeHolder.find('#saveButton');
			this.saveBtn.click($.proxy(this._saveProfile, this));
		},

		_saveProfile: function () {
			var profileName = this.placeHolder.find('#profileNameTextBox').val();
			if (profileName == "") return;

			var profile = { Name: profileName, Settings: {}};

			var requestUrl = '/api/v1/Plugins.asmx/{PluginName}/Profiles/{ProfileName}'.replace(/{PluginName}/g, this._pluginName).replace(/{ProfileName}/g, profileName);

			$.ajax({
				url: new Tp.WebServiceURL(requestUrl).url,
				type:'POST',
				dataType:"json",
				success:  $.proxy(function() {
					window.location.href = this._returnUrl;
                }, this),
				error: function (response) { alert(response); },
				data: JSON.stringify(profile)
			});
		}
	};

	new ProfileEditor({
		placeHolder: $('#' + config.placeholderId)
	}).render();
})
