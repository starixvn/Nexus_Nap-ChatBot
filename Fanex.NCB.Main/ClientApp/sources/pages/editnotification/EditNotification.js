import './EditNotification.scss';
import 'react-mde/lib/styles/css/react-mde-all.css';

import React from 'react';
import { Form, FormGroup, Label, Button, Col, Input, FormFeedback } from 'reactstrap';
import axios from 'axios';
import ReactMde from 'react-mde';
import { Formik } from 'formik';
import { object, string, array, date } from 'yup';
import { NCBBreadcrum } from '../common/breadcrumbs/breadcrumbs';
import TreeSelect, { getRealValuesFromExtra } from '../common/TreeSelect/index';
import DatePicker from '../common/DatePicker/index';
import Labels from '../common/Labels';
import { editorCommandList, invalidFieldClass } from '../constants/constants';
import ResponseMessage from '../common/response-message/ResponseMessage';
import Site from '../lib/site';
import ConfirmPopup from '../common/confirm-popup/ConfirmPopup';
import errorResponseHandler from '../lib/errorResponseHandler';
import ContentPreview from '../common/ContentPreview';
import MarkdownRules from '../common/markdown-rules';
import CommonFunction from '../common/common-functions/CommonFunctions';

const SendModeOptions = [
    {
        value: '1',
        label: 'Send Immediately'
    },
    {
        value: '2',
        label: 'Send At'
    }
];

const Today = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Taipei' }));

export default class EditNotification extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            notificationId: parseInt(document.getElementById('notification-id').value, 10),
            notification: null,
            recipientOptions: null,
            confimrationPopup: {
                isOpen: false,
                message: ''
            },
            markdownRulesPopup: {
                isOpen: false
            },
            searchValue: ''
        };

        this.debounceHandleSearchRecipents = CommonFunction.debounce(this.handleSearchRecipents);
        this.searchRecipients = [];
        this.allCheckedNodes = [];
    }

    componentDidMount() {
        const promises = [
            axios(Site.resolveClientUrl('EditNotification/Get/' + this.state.notificationId))
        ];

        Promise.all(promises)
            .then(([notificationResponse]) => {
                const notification = notificationResponse.data;
                notification.configSendTime = new Date(notification.configSendTime);

                this.setState({
                    notification
                }, this.getRecipients);
            }).catch(errorResponseHandler);
    }

    initializeObj = recipientOptions => {
        const recipientIds = [];
        this.findRecipientTreeValue(recipientIds, recipientOptions);
        const notification = this.state.notification;

        return {
            title: notification.title,
            content: notification.content,
            recipients: {
                value: recipientIds,
                allCheckedNodes: this.state.notification.recipientIds.map(x => ({ realValue: x }))
            },
            sendMode: notification.sendImmediately ? '1' : '2',
            sendDate: notification.configSendTime,
            productId: this.state.notification.productId || ''
        };
    }

    findRecipientTreeValue = (recipientIds, tree) => {
        for (let i = 0; i < tree.length; i++) {
            const item = tree[i];
            if (item.children && item.children.length > 0) {
                this.findRecipientTreeValue(recipientIds, item.children);
            } else {
                if (this.state.notification.recipientIds.some(r => r === item.realValue)) {
                    recipientIds.push(item.value);
                }
            }
        }
    }

    getRecipients = () => {
        const notification = this.state.notification;
        axios.get(Site.resolveClientUrl('EditNotification/GetRecipients'), {
            params: {
                notificationId: this.state.notificationId,
                notificationTypeId: notification.notificationTypeId,
                productId: notification.productId
            }
        }).then(response => {
            const initialObj = this.initializeObj(response.data);

            this.setState({ recipientOptions: response.data, initialObj: initialObj });
        }).catch(errorResponseHandler);
    }

    handleCloseConfirmationPopup = () => {
        this.refs.form.setSubmitting(false);
        this.setState({ confimrationPopup: { isOpen: false } });
    }

    handleConfirm = () => {
        this.setState({ confimrationPopup: { isOpen: false } });

        const formik = this.refs.form;
        const values = formik.state.values;

        const recipientIds = [];
        getRealValuesFromExtra(recipientIds, values.recipients.allCheckedNodes);

        const data = {
            notificationId: this.state.notificationId,
            recipientIds: recipientIds.toString(),
            title: values.title,
            content: values.content,
            sendDate: values.sendDate.toLocaleString('en-US'),
            sendImmediately: values.sendMode === '1'
        };

        this.handleCloseConfirmationPopup();

        axios({
            url: Site.resolveClientUrl('EditNotification/Edit'),
            method: 'post',
            data: data,
            headers: {
                'RequestVerificationToken': Site.getXsrfToken()
            }
        }).then(response => {
            const result = response.data;
            formik.setSubmitting(false);

            if (result.isSuccessful) {
                this.setState({ editNotificationResult: result },
                    () => setTimeout(function () {
                        window.location = Site.resolveClientUrl('NotificationList');
                    }, 3000));
            } else {
                this.setState({ editNotificationResult: result });
            }
        }).catch(errorResponseHandler);
    }

    getRecipientIds = (ids, collection) => {
        for (let i = 0; i < collection.length; i++) {
            const item = collection[i];
            if (item.children && item.children.length > 0) {
                this.getRecipientIds(ids, item.children);
            } else {
                ids.push(item.node.props.realValue);
            }
        }
    }

    schema = object().shape({
        title: string().required(Labels.FieldRequired),
        content: string().required(Labels.FieldRequired),
        recipients: object({
            value: array().required(Labels.FieldRequired)
        }),
        sendMode: string(),
        sendDate: date()
            .when('sendMode', (sendMode, schema) => sendMode === '2' ?
                schema.required(Labels.FieldRequired).min(Today, field => `Send date must be later than ${field.min.toLocaleString()}`) :
                schema)
    })

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>;

    handleSubmit = values => {
        const formatDate = values.sendDate.toLocaleString('en-US',
            { hour12: true, month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

        const message = values.sendMode === '1' ? 'Do you want to send the notification to the selected recipient(s) immediately?'
            : `Do you want to send the notification to the selected recipient(s)  at ${formatDate}?`;
        this.setState({ confimrationPopup: { isOpen: true, message: message } });
    }

    handleRecipientChange = (value, _label, extra) => {
        const formik = this.refs.form;
        formik.setFieldValue('recipients', { value, allCheckedNodes: extra.allCheckedNodes });
    }

    handleGerneratePreview = markdown => {
        return Promise.resolve(
            <ContentPreview text={markdown} />
        );
    }

    touchedAndErrors = (touched, errors, fieldName) => touched[fieldName] && errors[fieldName]

    handleOpenMarkdownRulesPopup = () => this.setState({ markdownRulesPopup: { isOpen: true } })

    handleCloseMarkdownRulesPopup = () => this.setState({ markdownRulesPopup: { isOpen: false } })

    onChangeSearch = value => {
        this.setState({ searchValue: value }, () => {
            if (this.state.recipientOptions) {
                this.debounceHandleSearchRecipents(value);
            }
        });
    }

    handleSearchRecipents = value => {
        let recipients = value.replace(/[,;\t]+/g, ',');
        recipients = recipients.split(',').map(recipient => recipient.trim());
        while (recipients.includes('')) {
            const index = recipients.indexOf('');
            recipients.splice(index, 1);
        }
        
        this.searchRecipients = [];
        this.allCheckedNodes = [];
        let validRecipients = recipients.filter(recipient => this.isValidSearchRecipients(recipient, this.state.recipientOptions[0]));
        while (validRecipients.length > 0) {
            const recipient = validRecipients.shift();
            const index = recipients.indexOf(recipient);
            recipients.splice(index, 1);
        }

        const formik = this.refs.form;
        const updateValues = [...formik.state.values.recipients.value,...this.searchRecipients];
        const updateAllCheckNodes = [...formik.state.values.recipients.allCheckedNodes,...this.allCheckedNodes];
        formik.setFieldValue('recipients', { value: updateValues, allCheckedNodes: updateAllCheckNodes});
        this.setState({ searchValue: recipients.join(',') });
    }

    isValidSearchRecipients = (label, recipient) => {
        if (recipient.title.toUpperCase() === label.toUpperCase()) {
            this.searchRecipients.push(recipient.value);
            this.allCheckedNodes.push({ realValue: recipient.realValue });
            return true;
        }
        else {
            if (recipient.children) {
                let children = recipient.children.map(child => this.isValidSearchRecipients(label, child));
                if (children.includes(true)) {
                    return true;
                }
            }
        }
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const isRecipientInvalid = this.touchedAndErrors(touched, errors, 'recipients');
        const isTitleInvalid = this.touchedAndErrors(touched, errors, 'title');
        const isContentInvalid = this.touchedAndErrors(touched, errors, 'content');
        const isSendDateInvalid = this.touchedAndErrors(touched, errors, 'sendDate');

        return (
            <Form onSubmit={handleSubmit} className="edit-notification-form">
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label required" sm={2}>Notification Type</Label>
                    <Col sm={10}>
                        <Input type="select" name="notificationTypeId" disabled
                            onChange={this.handleNotificationTypeChange}
                            value={this.state.notification.notificationTypeId}
                        >
                            <option value={this.state.notification.notificationTypeId} disabled hidden>{this.state.notification.notificationTypeName}</option>
                        </Input>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label" sm={2}>Product</Label>
                    <Col sm={10}>
                        <Input type="select"
                            value={values.productId}
                            name="productId" disabled
                            onChange={this.handleProductChange}
                        >
                            {values.productId ?
                                <option value={values.productId} disabled hidden>{this.state.notification.productName}</option>
                                : <option value="">{Labels.None}</option>
                            }
                        </Input>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label required" sm={2}>Recipients</Label>

                    <Col sm={10}>
                        <TreeSelect value={values.recipients.value} onChange={this.handleRecipientChange} placeholder={Labels.PleaseSelect}
                            treeData={this.state.recipientOptions} className={isRecipientInvalid && invalidFieldClass}
                            labelInValue={false} onSearch={this.onChangeSearch} searchValue={this.state.searchValue}
                        />
                        {isRecipientInvalid && <FormFeedback>{errors.recipients.value}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="title" className="form-label required" sm={2}>Title</Label>
                    <Col sm={10}>
                        <Input value={values.title} name="title"
                            invalid={isTitleInvalid} onChange={handleChange}
                        />
                        {isTitleInvalid && <FormFeedback>{errors.title}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="content" className="form-label required" sm={2}>Content</Label>
                    <Col sm={10}>
                        <ReactMde className={isContentInvalid && invalidFieldClass}
                            value={values.content} commands={editorCommandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400}
                        />
                        {isContentInvalid && <FormFeedback>{errors.content}</FormFeedback>}
                        <div className="markdown-rules" onClick={this.handleOpenMarkdownRulesPopup}>
                            Markdown Rules: <span className="icon-book" />
                        </div>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="sendMode" className="form-label required" sm={2}>Send Date/Time</Label>
                    <Col sm={10}>
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
                                        className={isSendDateInvalid && invalidFieldClass}
                                    />
                                    <span>(GMT +8)</span>
                                    {isSendDateInvalid && <FormFeedback>{errors.sendDate}</FormFeedback>}
                                </>
                            }
                        </div>
                    </Col>

                </FormGroup>
                <FormGroup row className="list-buttons">
                    <Button className="btn-submit offset-sm-2" disabled={isSubmitting}>{Labels.Submit}</Button>
                </FormGroup>
            </Form >
        );
    }

    render() {
        if (!this.state.notification || !this.state.recipientOptions) {
            return null;
        }

        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Edit Notification" />
                <div className="edit-notification">
                    {this.state.editNotificationResult &&
                        <ResponseMessage isSuccessful={this.state.editNotificationResult.isSuccessful}
                            message={this.state.editNotificationResult.message || Labels.EditNotificationSuccessfully} />
                    }

                    <Formik ref="form"
                        initialValues={this.state.initialObj}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                    <ConfirmPopup
                        onConfirm={this.handleConfirm}
                        onToggle={this.handleCloseConfirmationPopup}
                        isOpen={this.state.confimrationPopup.isOpen}
                        content={this.state.confimrationPopup.message}
                    />
                    {this.state.markdownRulesPopup.isOpen &&
                        <MarkdownRules show onToggle={this.handleCloseMarkdownRulesPopup} />
                    }
                </div>

            </>
        );
    }
}