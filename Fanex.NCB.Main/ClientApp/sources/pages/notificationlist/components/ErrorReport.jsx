import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { DateTimeFormat, NotificationStatus, invalidFieldClass } from '../../constants/constants';
import CommonFunctions from '../../common/common-functions/CommonFunctions';

import './ErrorReport.scss';

export default class ErrorReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            remark: '',
            remarkTouched: false
        };
    }

    handleRemarkChange = event => this.setState({ remark: event.target.value, remarkTouched: true });

    renderFailedRecipient = (recipient, index) => {
        return (
            <tr key={index}>
                <td>{recipient.recipientName}</td>
                <td>{recipient.recipientTypeName}</td>
                <td>{recipient.groupName}</td>
                <td className="text-red">{recipient.errorMessage}</td>
            </tr>
        );
    };

    copyToClipboard = () => {
        const input = document.createElement('input');
        input.value = this.props.report.errorDetails.map(x => x.recipientName).join(', ');
        document.body.appendChild(input);
        input.select();
        document.execCommand('copy');
        document.body.removeChild(input);
        CommonFunctions.displayCopiedToClipboard();
    };

    handleResolveNotification = () => {
        if (this.state.remark !== '') {
            this.props.onResolveNotification(this.state.remark.trim());
        } else {
            if (!this.state.remarkTouched) {
                this.setState({ remarkTouched: true });
            }
        }
    };

    render() {
        const { isOpen, report, toggle, haveHostPermission } = this.props;

        if (report) {
            return (
                <Modal isOpen={isOpen} toggle={toggle} className="error-report" backdrop="static">
                    <ModalHeader toggle={toggle}>Notification Sending Error Report</ModalHeader>
                    <ModalBody>
                        <p>
                            Error report for the notification titled:<br />
                            <span className="bold text-uppercase">{report.title}</span>
                        </p>
                        <p>
                            Send at:<br />
                            <span className="bold">{report.configSendTime.toFormat(DateTimeFormat)}</span>
                        </p>

                        {report.successRecipients.length > 0 &&
                            <div className="msg-success">
                                Notification has been successfully sent to the following recipient(s): <strong>{report.successRecipients.join(', ')}</strong>
                            </div>
                        }

                        {report.errorDetails.length > 0 &&
                            <>
                                <div className="msg-error">Error(s) occoured while sending the notification to the following recipient(s)</div>
                                <table className="tbl w-100">
                                    <thead>
                                        <tr>
                                            <th>
                                                <div className="has-icons">Recipient<span className="icon-copy" onClick={this.copyToClipboard} /></div>
                                            </th>
                                            <th>Recipient Type</th>
                                            <th>Group Name</th>
                                            <th>Reason</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {report.errorDetails.map(this.renderFailedRecipient)}
                                    </tbody>
                                </table>
                            </>
                        }

                        {report.statusId === NotificationStatus.Error && haveHostPermission &&
                            <div className="remark">
                                <span>Remark:</span>
                                <input type="text" className={`form-control ${this.state.remarkTouched && this.state.remark === '' && invalidFieldClass}`}
                                    onChange={this.handleRemarkChange}
                                    value={this.state.remark}
                                    placeholder="Input the remark here..."
                                />
                                <button className="btn-mark" onClick={this.handleResolveNotification} disabled={this.props.isSubmitting}>Mark As Resolved</button>
                            </div>
                        }
                        {report.statusId === NotificationStatus.Resolved &&
                            <div className="resolved-remark">
                                <p>Remark: <span className="text-italic">{report.resolvedRemark}</span></p>
                                <p><span className="text-red">*</span> Resolved by: <strong>{report.resolvedByUserName}</strong></p>
                            </div>
                        }
                    </ModalBody>
                </Modal>
            );
        }

        return null;
    }
}