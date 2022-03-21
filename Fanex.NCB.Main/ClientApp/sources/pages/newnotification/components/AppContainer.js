import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React from 'react';
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
            initValue: {
                channelIds: `${Channels.Skype},${Channels.Telegram}`,
                channelName: '',
                notificationTypeId: '',
                templateId: '',
                productId: '',
                platforms: '',
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
        this.searchPlatforms = [];

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
        this.props.getResendNotification(notificationId, '', isResendAll).then(response => {
            const { resendNotification, recipients, unSelectableRecipients } = response.data;
            this.props.setRecipients({ recipients: recipients});
            this.selectedRecipientIds = resendNotification.recipients.map(r => r.recipientId);
            const selectedRecipients = resendNotification.recipients.map(r => {
                return {
                    label: r.recipientName,
                    value: r.licenseeTypeId ?
                        r.recipientTypeId + '-' + r.licenseeTypeId + '-' + r.recipientId :
                        r.recipientTypeId + '-' + r.recipientId
                };
            });
            this.props.getPlatforms(resendNotification.notificationTypeId).then(res => {
                let selectedPlatforms = [];
                let platforms = '';

                if (res.data.length> 0) {
                    selectedPlatforms = res.data.map(item => ({ id: item.platformId, name: item.platformName }));
                }

                if (resendNotification.platforms.length > 0) {
                    platforms = resendNotification.platforms.map(item => item.platformId).join(',');
                }

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
                        platforms: platforms
                    },
                    platformOptions: selectedPlatforms
                });
                this.displayUnselectableRecipients(unSelectableRecipients);
            });
        });
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
            platforms: values.platforms
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
        if (!isValidContentLength) {
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

    handleNotificationTypeChange = event => {
        const value = event.target.value;
        const formik = this.refs.form;
        var platforms = '';
        this.props.getPlatforms(event.target.value).then(response => {
            if (response.data.length > 0) {
                platforms = response.data.map(item => item.platformId).join(',');
                const platformOptions = response.data.map(item => { return { id: item.platformId, name: item.platformName, checked: true }});
                this.setState({ platformOptions: platformOptions });
            } else {
                this.setState({ platformOptions: [] });
            }

            formik.setFieldValue('platforms', platforms);
            this.props.getRecipients(value, platforms, '',formik.state.values.productId, formik.state.values.channelIds);
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
        });
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

        var platforms = '';
        if (formik.state.values.platforms.length > 0) {
            platforms = formik.state.values.platforms.map(item => item.value).join(',');
        }

        await this.props.getRecipients(formik.state.values.notificationTypeId, platforms, value, formik.state.values.channelIds);

        if (formik.state.values.recipients.length > 0) {
            formik.setFieldValue('recipients', []);
        }

        if (!this.props.recipientOptions.find(item => parseInt(item.value, 10) === 1)) {
            formik.setFieldValue('platforms', []);
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

        await this.props.getRecipients(formik.state.values.notificationTypeId, value, recipients,formik.state.values.productId, formik.state.values.channelIds);

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

    handleOpenMarkdownRulesPopup = () => this.props.setMarkdownRulesPopup({ isOpen: true });

    handleCloseMarkdownRulesPopup = () => this.props.setMarkdownRulesPopup({ isOpen: false });

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
        const platformOptions = this.state.platformOptions.map(item => {
            return {
                id: item.id,
                name: item.name,
                checked: values.platforms.split(',').includes(item.id.toString())
            }
        });

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
                    <Label for="notificationTypeId" className="form-label">Notification Template</Label>
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
                    <FormGroup>
                        <fieldset className="parameter-area">
                            <legend>Parameters</legend>
                            <FormGroup className="parameters">
                                {values.params.map((x, i) =>
                                (
                                    <div className="parameter-col">
                                        <Label for={`params.${i}.value`} className="form-label">{x.name}</Label>
                                        <div>
                                            <Input value={x.values} name={`params.${i}.value`} disabled={values.mode === 'preview'}
                                                onChange={handleChange} onBlur={this.handleReplaceParamsWithValue}
                                            />
                                        </div>
                                    </div>
                                )
                                )}
                            </FormGroup>
                        </fieldset>
                    </FormGroup>
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
                <NCBBreadcrum chatbotName={Labels.ChatbotSystem} pageTitle={pageTitle} />
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
        markdownRulesPopup: state.markdownRulesPopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);