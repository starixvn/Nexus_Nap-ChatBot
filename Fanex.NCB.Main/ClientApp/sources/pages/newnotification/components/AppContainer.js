import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Fragment } from 'react';
import { connect } from 'react-redux';
import { DateTime } from 'luxon';
import { Form, FormGroup, Label, Button, Input, FormFeedback } from 'reactstrap';
import { stringify } from 'qs';
import { Formik } from 'formik';
import { object, string, array, date } from 'yup';
import ReactMde from 'react-mde';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import TreeSelect, { getRealValueFromTreeSelection } from '../../common/TreeSelect/index';
import DatePicker from '../../common/DatePicker/index';
import Labels from '../../common/Labels';
import { DateTimeFormat, editorCommandList, invalidFieldClass, Channels } from '../../constants/constants';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import Site from '../../lib/site';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import ContentPreview from '../../common/ContentPreview';
import MarkdownRules from '../../common/markdown-rules';
import CommonFunction from '../../common/common-functions/CommonFunctions';
import MultipleSelect from '../../common/multiple-select/index';
import UserGuide from '../../common/user-guide';

const SendModeOptions = [
    { value: '1', label: 'Send Immediately' },
    { value: '2', label: 'Send At' }
];

const Today = new Date(new Date().toLocaleString("en-US", { timeZone: "Asia/Taipei" }));

export class AppContainer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            isSummited: false,
            manuallyEditted: false,
            noFilledParams: [],
            template: {
                title: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                content: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                params: []
            },
            searchValue: '',
            searchPlatformValue: '',
            platformOptions: [],
            integrationMethodOptions: [],
            initValue: {
                channelIds: `${Channels.Skype},${Channels.Telegram}`,
                channelName: '',
                notificationTypeId: '',
                templateId: '',
                productId: '',
                platforms: '',
                integrationMethodIds: '',
                fileName: null,
                recipients: [],
                title: '',
                content: '',
                sendDate: Today,
                sendMode: '1',
                params: [],
                mode: 'write',
                canEditTitleContent: false
            }
        };

        this.messageContentLength = parseInt(document.getElementById('MessageContentLength').value, 10);
        this.debounceHandleSearchRecipents = CommonFunction.debounce(this.handleSearchRecipents);
        this.searchRecipients = [];

        const windowUrl = window.location.search;
        const params = new URLSearchParams(windowUrl);

        this.resendNotifcation = {
            notificationId: params.get('notificationId'),
            isResendAll: params.get('isResendAll')
        };
    }

    componentDidMount() {
        this.props.getProducts();
        this.props.getNotificationTypes();
        if (this.resendNotifcation.notificationId) {
            this.handleInitResendNotification();
        }
    }

    handleInitResendNotification = () => {
        const { notificationId, isResendAll } = this.resendNotifcation;
        this.props.getResendNotification(notificationId, '', isResendAll).then(async response => {
            const { resendNotification, recipients, unSelectableRecipients } = response.data;
            this.props.setRecipients({ recipients: recipients });
            this.selectedRecipientIds = resendNotification.recipients.map(r => r.recipientId);
            const selectedRecipients = resendNotification.recipients.map(r => {
                return {
                    label: r.recipientName,
                    value: r.licenseeTypeId ?
                        r.recipientTypeId + '-' + r.licenseeTypeId + '-' + r.recipientId :
                        r.recipientTypeId + '-' + r.recipientId
                };
            });

            var platforms = await this.getResendOptions(this.props.getPlatforms, resendNotification, 'platforms', { id: 'platformId', name: 'platformName'});
            var integrationMethods = await this.getResendOptions(this.props.getIntegrationMethods, resendNotification, 'integrationMethods', { id: 'integrationMethodId', name: 'integrationMethodName'});

            this.setState({
                initValue: {
                    ...this.state.initValue,
                    channelIds: resendNotification.channelId.toString(),
                    channelName: resendNotification.channelName,
                    notificationTypeId: resendNotification.notificationTypeId,
                    recipients: selectedRecipients,
                    title: resendNotification.title,
                    content: resendNotification.content,
                    sendDate: Today,
                    sendMode: '1',
                    productId: this.props.products.find(x => x.id === resendNotification.productId) ? resendNotification.productId : '',
                    platforms: platforms.data,
                    integrationMethodIds: integrationMethods.data,
                    fileName: resendNotification.fileName
                },
                platformOptions: platforms.options,
                integrationMethodOptions: integrationMethods.options
            });
            this.displayUnselectableRecipients(unSelectableRecipients);
        });
    };

    getResendOptions = async (propsFunc, resendNotification, values, key) => {
        var data = '', options = [];
        await propsFunc(resendNotification.notificationTypeId).then(res => {
            if (res.data.length > 0) {
                options = res.data.map(item => ({ id: item[`${key.id}`], name: item[`${key.name}`] }));
            }

            if (resendNotification[`${values}`].length > 0) {
                data = resendNotification[`${values}`].map(item => item[`${key.id}`]).join(',');
            }
        });
        return { data, options };
    };

    displayUnselectableRecipients = recipients => {
        if (recipients && recipients.length > 0) {
            const html = `<div class='warning-title'>
                            <span class='icon-warning'></span> Unselectable recipient(s): `+ recipients.map(r => r.recipientName).join(', ') + `.</div>`;
            document.getElementById('header-text').innerHTML = html;
        }
    };

    handleToggleConfirmationPopup = () => {
        this.props.setConfirmationPopup({ isOpen: !this.props.confirmationPopup.isOpen });
        this.refs.form.setSubmitting(false);
    };

    handleToggleResetConfirmationPopup = () => {
        this.props.setResetConfirmationPopup({ isOpen: false });
    };

    handleToggleEditConfirmationPopup = () => {
        this.props.setEditConfirmationPopup({ isOpen: false });
    };

    handleConfirm = () => {
        this.props.setConfirmationPopup({ isOpen: false });

        const formik = this.refs.form;
        const values = formik.state.values;
        const recipientIds = [];
        getRealValueFromTreeSelection(recipientIds, values.recipients, this.props.recipientOptions);

        const data = stringify({
            channelIds: values.channelIds,
            notificationTypeId: values.notificationTypeId,
            productId: values.productId,
            recipientIds: recipientIds.toString(),
            title: values.title.trim(),
            content: values.content.trim(),
            sendDate: DateTime.fromJSDate(values.sendDate).toFormat(DateTimeFormat),
            sendImmediately: values.sendMode === '1',
            platforms: values.platforms,
            integrationMethodIds: values.integrationMethodIds,
            fileName: values.fileName
        });

        this.props.createNewNotification(data).then(() => {
            formik.setSubmitting(false);
            if (this.props.createNotificationResult.isSuccessful) {
                setTimeout(function () {
                    window.location = Site.resolveClientUrl('NotificationList');
                }, 3000);
            }
        });
    };

    schema = object().shape({
        notificationTypeId: string().required(Labels.FieldRequired),
        title: string().required(Labels.FieldRequired),
        content: string().required(Labels.FieldRequired),
        recipients: array().required(Labels.FieldRequired),
        sendMode: string(),
        sendDate: date()
            .when('sendMode', (sendMode, schema) => sendMode === '2' ?
                schema.required(Labels.FieldRequired).min(Today, field => `Send date must be later than ${field.min.toLocaleString()}`)
                : schema)
    });

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderNotificationTemplateOption = item => <option key={item.notificationTemplateId} value={item.notificationTemplateId} >{item.notificationTemplateName}</option>

    renderProductOption = item => <option key={item.id} value={item.id} >{item.name}</option>

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>;

    handleSubmit = values => {
        const isValidContentLength = values.content.trim().length <= this.messageContentLength;
        const isValidParams = this.isValidParams(values);
        if (!isValidContentLength || !isValidParams) {
            const formik = this.refs.form;
            formik.setSubmitting(false);
            return;
        }

        const formatDate = values.sendDate.toLocaleString('en-US',
            { hour12: true, month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

        const message = values.sendMode === '1' ? 'Do you want to send the notification to the selected recipient(s) immediately?'
            : `Do you want to send the notification to the selected recipient(s)  at ${formatDate}?`;
        this.props.setConfirmationPopup({ isOpen: true, content: message });
    };

    isValidParams = values => {
        if (values.canEditTitleContent)
            return true;
    
        const params = values.params;
        const notFilledParams = params.filter(p => p.value === '');
        this.setState({ noFilledParams: notFilledParams });

        return notFilledParams.length === 0;
    }

    getPlatformCreateOptions = async (value, formik) => {
        await this.props.getPlatforms(value).then(response => {
            var platforms = '';
            if (response.data.length > 0) {
                platforms = response.data.map(item => item.platformId).join(',');
                const platformOptions = response.data.map(item => { return { id: item.platformId, name: item.platformName, checked: true } });
                this.setState({ platformOptions: platformOptions });
            } else {
                this.setState({ platformOptions: [] });
            }
            formik.setFieldValue('platforms', platforms);
        });
    };

    getIntegrationCreateOptions = async (value, formik) => {
        await this.props.getIntegrationMethods(value).then(response => {
            var integrationMethodIds = '';
            if (response.data.length > 0) {
                integrationMethodIds = response.data.map(item => item.integrationMethodId).join(',');
                const integrationMethodOptions = response.data.map(item => { return { id: item.integrationMethodId, name: item.integrationMethodName, checked: true } });
                this.setState({ integrationMethodOptions: integrationMethodOptions });
            } else {
                this.setState({ integrationMethodOptions: [] });
            }
            formik.setFieldValue('integrationMethodIds', integrationMethodIds);
        });
    };

    handleNotificationTypeChange = async event => {
        const value = event.target.value;
        const formik = this.refs.form;
        await this.getPlatformCreateOptions(value, formik);
        await this.getIntegrationCreateOptions(value, formik);

        this.props.getRecipients(value, formik.state.values.platforms, '', formik.state.values.productId, formik.state.values.integrationMethodIds, formik.state.values.channelIds);
        this.props.getNotificationTemplates(value);
        formik.setValues({
            ...formik.state.values,
            notificationTypeId: value,
            recipients: [],
            templateId: '',
            params: [],
            title: '',
            content: ''
        });
        this.setState({ searchValue: '', searchPlatformValue: '' });
    };

    handleTemplateChange = event => {
        const formik = this.refs.form;

        if (event.target.value) {
            const selectedTemplate = this.props.notificationTemplates
                .find(x => x.notificationTemplateId === parseInt(event.target.value, 10));

            const paramsInTitle = selectedTemplate.title.match(/{[_-\w\s]+}/g) || [];
            const paramsInContent = selectedTemplate.content.match(/{[_-\w\s]+}/g) || [];
            const paramNames = new Set([...paramsInTitle, ...paramsInContent]);

            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [...paramNames].map(x => ({ name: x, value: '' })),
                title: selectedTemplate.title,
                content: selectedTemplate.content,
                canEditTitleContent: false
            });
        } else {
            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [],
                title: '',
                content: '',
                canEditTitleContent: true
            });
        }
    };

    handleProductChange = async event => {
        const formik = this.refs.form;
        const value = event.target.value;
        formik.setFieldValue('productId', value);

        var platforms = formik.state.values.platforms;
        var recipients = '';
        if (formik.state.values.recipients.length > 0) {
            recipients = formik.state.values.recipients.map(item => this.getRealValue(item.value)).join(',');
        }

        await this.props.getRecipients(formik.state.values.notificationTypeId, platforms, recipients, value, formik.state.values.integrationMethodIds, formik.state.values.channelIds);

        if (formik.state.values.recipients.length > 0) {
            const valuesRecipients = formik.state.values.recipients.filter(item => this.props.selectedRecipients.includes(this.getRealValue(item.value)));
            formik.setFieldValue('recipients', valuesRecipients);
        }
    };

    getRealValue = value => {
        const arrValue = value.split('-');
        const arrLength = arrValue.length - 1;
        return parseInt(arrValue[arrLength], 10);
    };

    handlePlatformChange = async value => {
        const formik = this.refs.form;
        formik.setFieldValue('platforms', value);

        var recipients = '';
        if (formik.state.values.recipients.length > 0) {
            recipients = formik.state.values.recipients.map(item => this.getRealValue(item.value)).join(',');
        }

        await this.props.getRecipients(formik.state.values.notificationTypeId, value, recipients, formik.state.values.productId, formik.state.values.integrationMethodIds, formik.state.values.channelIds);

        if (formik.state.values.recipients.length > 0) {
            const valuesRecipients = formik.state.values.recipients.filter(item => this.props.selectedRecipients.includes(this.getRealValue(item.value)));
            formik.setFieldValue('recipients', valuesRecipients);
        }
    };

    handleIntegrationMethodChange = async value => {
        const formik = this.refs.form;
        formik.setFieldValue('integrationMethodIds', value);

        var recipients = '';
        if (formik.state.values.recipients.length > 0) {
            recipients = formik.state.values.recipients.map(item => this.getRealValue(item.value)).join(',');
        }

        await this.props.getRecipients(formik.state.values.notificationTypeId, formik.state.values.platforms, recipients, formik.state.values.productId, value, formik.state.values.channelIds);

        if (formik.state.values.recipients.length > 0) {
            const valuesRecipients = formik.state.values.recipients.filter(item => this.props.selectedRecipients.includes(this.getRealValue(item.value)));
            formik.setFieldValue('recipients', valuesRecipients);
        }
    };

    handleRecipientChange = (value, label, extra) => {
        const formik = this.refs.form;
        formik.setFieldValue('recipients', value);
    };

    handleResetTitleContent = () => {
        const message = 'Reset Title and Content will recover the original template and discard your changes.';
        this.props.setResetConfirmationPopup({ isOpen: true, content: message });
    };

    handleConfirmResetTitleContent = () => {
        this.props.setResetConfirmationPopup({ isOpen: false });

        const formik = this.refs.form;
        const selectedTemplate = this.props.notificationTemplates
            .find(x => x.notificationTemplateId === parseInt(formik.state.values.templateId, 10));

        formik.setValues({
            ...formik.state.values,
            title: selectedTemplate.title,
            content: selectedTemplate.content,
            canEditTitleContent: false,
            params: formik.state.values.params.map(x => ({ name: x.name, value: '' }))
        });
    };

    handleEditTitleContent = () => {
        const message = 'You can edit the Title and Content but the system doesn\'t support manually change the text in {}';
        this.props.setEditConfirmationPopup({ isOpen: true, content: message });
    };

    handleConfirmEditTitleContent = () => {
        this.props.setEditConfirmationPopup({ isOpen: false });
        const formik = this.refs.form;
        formik.setFieldValue('canEditTitleContent', true);
    };

    handleReplaceParamsWithValue = () => {
        const formik = this.refs.form;
        const params = formik.state.values.params;
        const selectedTemplate = this.props.notificationTemplates
            .find(x => x.notificationTemplateId === parseInt(formik.state.values.templateId, 10));

        let { title, content } = selectedTemplate;

        for (let i = 0; i < params.length; i++) {
            if (params[i].value) {
                title = title.replace(new RegExp(`\\${params[i].name}`, 'g'), params[i].value);
                content = content.replace(new RegExp(`\\${params[i].name}`, 'g'), params[i].value);
            }
        }

        formik.setValues({
            ...formik.state.values,
            title,
            content
        });
    };

    handleGerneratePreview = markdown => {
        return Promise.resolve(
            <ContentPreview text={markdown} />
        );
    };

    onChangeSearch = value => {
        this.setState({ searchValue: value }, () => {
            if (this.props.recipientOptions) {
                this.debounceHandleSearchRecipents(value);
            }
        });
    };

    handleSearchRecipents = value => {
        let recipients = value.replace(/[,;\t]+/g, ',');
        recipients = recipients.split(',').map(recipient => recipient.trim());
        while (recipients.includes('')) {
            const index = recipients.indexOf('');
            recipients.splice(index, 1);
        }

        this.searchRecipients = [];
        const validRecipients = [];
        for (let i = 0; i < this.props.recipientOptions.length; i++) {
            recipients.filter(recipient => this.isValidSearchRecipients(validRecipients, recipient, this.props.recipientOptions[i]));
        };

        while (validRecipients.length > 0) {
            const recipient = validRecipients.shift();
            const index = recipients.indexOf(recipient);
            recipients.splice(index, 1);
        }

        const formik = this.refs.form;
        const updateValues = [...formik.state.values.recipients, ...this.searchRecipients];
        formik.setFieldValue('recipients', updateValues);
        this.setState({ searchValue: recipients.join(',') });
    };

    isValidSearchRecipients = (result, label, recipient) => {
        if (recipient.title.toUpperCase() === label.toUpperCase()) {
            result.push(label);
            this.searchRecipients.push({
                label: label,
                value: recipient.value
            });

            return true;
        }
        else {
            if (recipient.children) {
                let children = recipient.children.map(child => this.isValidSearchRecipients(result, label, child));
                if (children.includes(true)) {
                    return true;
                }
            }
        }
    };

    getOptionChange = (data, options) => {
        return options.map(item => {
            return {
                id: item.id,
                name: item.name,
                checked: data.split(',').includes(item.id.toString())
            }
        });
    };

    handleOpenMarkdownRulesPopup = () => this.props.setMarkdownRulesPopup({ isOpen: true });

    handleCloseMarkdownRulesPopup = () => this.props.setMarkdownRulesPopup({ isOpen: false });

    handleOpenUserGuidePopup = () => this.props.setUserGuidePopup({ isOpen: true });

    handleCloseUserGuidePopup = () => this.props.setUserGuidePopup({ isOpen: false });

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const isValidContentLength = values.content.trim().length <= this.messageContentLength;
        const platformOptions = this.getOptionChange(values.platforms, this.state.platformOptions);
        const integrationMethodOptions = this.getOptionChange(values.integrationMethodIds, this.state.integrationMethodOptions);
        const isHaveOneParam = values.params.length === 1;
        const noFilledParams = this.state.noFilledParams;

        return (
            <Form onSubmit={handleSubmit} className="new-notification-form">
                {
                    this.resendNotifcation.notificationId &&
                    <FormGroup>
                        <Label className="form-label">{Labels.BotService}</Label>
                        <div>{values.channelName}</div>
                    </FormGroup>
                }
                <FormGroup>
                    <Label for="notificationTypeId" className="form-label required">{Labels.NotificationType}</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="productId" className="form-label">Product</Label>
                    <div>
                        <Input type="select"
                            value={values.productId}
                            name="productId"
                            onChange={this.handleProductChange}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.products.map(this.renderProductOption)}
                        </Input>
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="platforms" className="form-label">Platform</Label>
                    <MultipleSelect
                        dataSource={platformOptions}
                        hasNoneOption
                        keyField='id' valueField='name'
                        maxDisplayItemCount={2}
                        hasAllOption={platformOptions.length > 0}
                        onChange={(currentItem, selectedItemsKey) => {
                            this.handlePlatformChange(selectedItemsKey);
                        }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label for="integrationMethods" className="form-label">Integration Method</Label>
                    <MultipleSelect
                        dataSource={integrationMethodOptions}
                        hasNoneOption
                        keyField='id' valueField='name'
                        maxDisplayItemCount={2}
                        hasAllOption={integrationMethodOptions.length > 0}
                        onChange={(currentItem, selectedItemsKey) => {
                            this.handleIntegrationMethodChange(selectedItemsKey);
                        }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label for="productId" className="form-label required">Recipients</Label>
                    <div className="new-notification-select-tree">
                        <TreeSelect value={values.recipients} onChange={this.handleRecipientChange} placeholder={Labels.PleaseSelect}
                            treeData={this.props.recipientOptions} className={touched.recipients && errors.recipients && invalidFieldClass}
                            onSearch={this.onChangeSearch} searchValue={this.state.searchValue} treeDefaultExpandAll
                            defaultValue={this.state.initValue.recipients}
                        />
                        {touched.recipients && errors.recipients && <FormFeedback>{errors.recipients}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className="row-has-reset">
                    <Label for="notificationTemplate" className="form-label">Notification Template</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.templateId && errors.templateId}
                            name="templateId"
                            onChange={this.handleTemplateChange}
                            value={values.templateId}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.notificationTemplates.map(this.renderNotificationTemplateOption)}
                        </Input>
                        {touched.templateId && errors.templateId && <FormFeedback>{errors.templateId}</FormFeedback>}
                    </div>
                    {values.templateId &&
                        <div className="action">
                            <div className="circle orange" title="Edit Title & Content" onClick={this.handleEditTitleContent}  >
                                <span className="icon-edit" />
                            </div>
                            <div className="circle blue" title="Reset Title & Content" onClick={this.handleResetTitleContent}  >
                                <span className="icon-reset" />
                            </div>
                        </div>
                    }
                </FormGroup>
                <FormGroup>
                    <Label for="title" className="form-label required">Title</Label>
                    <div>
                        <Input value={values.title} name="title" autoComplete="off"
                            invalid={touched.title && errors.title}
                            onChange={handleChange} readOnly={!values.canEditTitleContent && values.templateId}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                        {
                            !values.canEditTitleContent && noFilledParams.length > 0 && noFilledParams.some(p => values.title.includes(p.name)) &&
                            <FormFeedback>{Labels.NoFullFillParamsErrMsg}</FormFeedback>
                        }
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="content" className="form-label required">Content</Label>
                    <div>
                        <ReactMde className={(touched.content && errors.content) || !isValidContentLength ? invalidFieldClass : ''}
                            value={values.content} commands={editorCommandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400} readOnly={!values.canEditTitleContent && values.templateId}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                        {
                            !values.canEditTitleContent && noFilledParams.length > 0 && noFilledParams.some(p => values.content.includes(p.name)) &&
                            <FormFeedback>{Labels.NoFullFillParamsErrMsg}</FormFeedback>
                        }
                        <div className="extension-content-section">
                            <div className="markdown-rules" onClick={this.handleOpenMarkdownRulesPopup}>
                                Markdown Rules: <span className="icon-book" />
                            </div>
                            <div className={`content-counter ${isValidContentLength ? '' : 'invalid'}`}>
                                Maximum Length {values.content.trim().length}/{this.messageContentLength}
                            </div>
                        </div>

                    </div>

                </FormGroup>
                {values.params.length > 0 && !values.canEditTitleContent &&
                    <Fragment>
                        <FormGroup>
                            <fieldset className="parameter-area">
                                <legend>Parameters</legend>
                                <FormGroup className={`parameters${isHaveOneParam ? ' block' : ''}`}>
                                    {values.params.map((x, i) =>
                                    (
                                        <div className="parameter-col">
                                            <Label for={`params.${i}.value`} className="form-label">{x.name}</Label>
                                            <div>
                                                <textarea value={x.values} name={`params.${i}.value`} disabled={values.mode === 'preview'} className="form-control"
                                                    rows={`${isHaveOneParam ? 4 : 2}`}
                                                    onChange={handleChange} onBlur={this.handleReplaceParamsWithValue}
                                                />
                                            </div>
                                        </div>
                                    )
                                    )}
                                </FormGroup>
                            </fieldset>
                        </FormGroup>
                    </Fragment>
                }
                <FormGroup>
                    <Label for="sendMode" className="form-label required">Send Date/Time</Label>
                    <div>
                        <div className="send-date-area">
                            <Input type='select'
                                value={values.sendMode}
                                name="sendMode"
                                onChange={handleChange}
                            >
                                {SendModeOptions.map(this.renderSendModeOption)}
                            </Input>
                            {values.sendMode === '2' &&
                                <>
                                    <DatePicker selected={values.sendDate} showTimeSelect
                                        onChange={value => setFieldValue('sendDate', value)} minDate={Today}
                                        className={touched.sendDate && errors.sendDate && invalidFieldClass}
                                    />
                                    <span>(GMT +8)</span>
                                    {touched.sendDate && errors.sendDate && <FormFeedback>{errors.sendDate}</FormFeedback>}
                                </>
                            }
                        </div>
                    </div>
                </FormGroup>
                <div className="list-buttons">
                    <Button className="btn-submit" disabled={isSubmitting} type="submit">{Labels.Submit}</Button>
                </div>
            </Form >
        );
    }

    renderPageTitle = () => this.resendNotifcation.notificationId ? "Resend a notification from notification #" + this.resendNotifcation.notificationId : "New Notification";

    render() {
        if (!this.props.notificationTypes || !this.props.products) {
            return null;
        }

        const initValue = this.state.initValue;
        const pageTitle = this.renderPageTitle();
        return (
            <>
                <div className="new-guide">
                    <NCBBreadcrum chatbotName={Labels.ChatbotSystem} pageTitle={pageTitle} />
                    <span className="icon-guide" onClick={this.handleOpenUserGuidePopup} />
                </div>
                <div className="new-notification">
                    {
                        this.props.createNotificationResult &&
                        <ResponseMessage isSuccessful={this.props.createNotificationResult.isSuccessful}
                            message={this.props.createNotificationResult.message || Labels.CreateNotificationSuccessfully} />
                    }

                    <Formik ref="form"
                        enableReinitialize
                        initialValues={initValue}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                </div>
                <ConfirmPopup
                    onConfirm={this.handleConfirm}
                    onToggle={this.handleToggleConfirmationPopup}
                    isOpen={this.props.confirmationPopup.isOpen}
                    content={this.props.confirmationPopup.content}
                />
                <ConfirmPopup
                    onConfirm={this.handleConfirmResetTitleContent}
                    onToggle={this.handleToggleResetConfirmationPopup}
                    isOpen={this.props.resetConfirmationPopup.isOpen}
                    content={this.props.resetConfirmationPopup.content}
                    warningIcon
                />
                <ConfirmPopup
                    onConfirm={this.handleConfirmEditTitleContent}
                    onToggle={this.handleToggleEditConfirmationPopup}
                    isOpen={this.props.editConfirmationPopup.isOpen}
                    content={this.props.editConfirmationPopup.content}
                    warningIcon
                />
                {
                    this.props.markdownRulesPopup.isOpen &&
                    <MarkdownRules show onToggle={this.handleCloseMarkdownRulesPopup} />
                }
                {
                    this.props.userGuidePopup.isOpen &&
                    <UserGuide show onToggle={this.handleCloseUserGuidePopup} />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        notificationTypes: state.notificationTypes,
        products: state.products,
        recipientOptions: state.recipients.recipientOptions,
        selectedRecipients: state.recipients.selectedRecipients,
        createNotificationResult: state.createNotificationResult,
        confirmationPopup: state.confirmationPopup,
        notificationTemplates: state.notificationTemplates,
        resetConfirmationPopup: state.resetConfirmationPopup,
        editConfirmationPopup: state.editConfirmationPopup,
        markdownRulesPopup: state.markdownRulesPopup,
        userGuidePopup: state.userGuidePopup,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);