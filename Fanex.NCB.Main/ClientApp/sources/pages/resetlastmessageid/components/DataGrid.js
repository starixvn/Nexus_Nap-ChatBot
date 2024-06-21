import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Input } from 'reactstrap';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import { LastVoidingHistoryCollectionId, LastMsgTargetId, DateTimeFormatV3 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            checked: [],
            allChecked: false,
            isSubmitting: true,
            isShowConfirmPopup: false,
            editResult: null
        };
    }

    componentDidMount() {
        this.getAllData();
    }

    getAllData = async () => {
        await this.props.getCurrentPreviousMaxMessage();

        const { currentMessages, selected } = this.props;
        const checked = [];

        const newMaxMessageIds = selected.map((item, index) => {
            const element = { ...item };

            const newMaxMessageId = currentMessages.data.find(el => el.jobIdentify === item.id && el.settingName === item.settingName);
            element.newMaxMessageId = newMaxMessageId.maxReferenceId;

            const notificationId = element.id === 9 && currentMessages.missingMsgInfo.find(item => dateFormat(item.postTime, DateTimeFormatV3) ===  newMaxMessageId.maxReferenceId && item.jobIdentify === 9).notificationId;
            element.notificationId = notificationId;

            checked.push(false);

            return element;
        });

        this.setState({checked});
        this.props.setSelected(newMaxMessageIds);
    };

    isNotEqualCurrentNew = (jobIdentify, settingName) => {
        const { currentMessages } = this.props;
        var itemCurrentMessages = currentMessages.data.find(item => item.jobIdentify === jobIdentify && item.settingName === settingName);
        if (itemCurrentMessages?.maxReferenceId !== itemCurrentMessages?.referenceValue) {
            return true;
        }
        return false;
    };

    filterDataJobInternalCSD = (data, jobIdentify, settingName) => {
        if (jobIdentify === 2 && settingName === LastMsgTargetId) {
            data = data.filter(x => x.msgTargetId > 0);
        }
        if (jobIdentify === 2 && settingName === LastVoidingHistoryCollectionId) {
            data = data.filter(x => x.voidingHistoryCollectionId > 0);
        }
        return data;
    };

    handleShowEditNewMessage = (jobIdentify, settingName) => {
        const itemSelected = this.props.selected.find(item => item.id === jobIdentify && item.settingName === settingName);
        const title = itemSelected?.name;
        const newMaxMessageId = itemSelected?.newMaxMessageId;
        let data;
        data = this.props.currentMessages.missingMsgInfo.filter(x => x.jobIdentify === jobIdentify);
        data = this.filterDataJobInternalCSD(data, jobIdentify, settingName);
        this.props.setEditNewMessagePopup({ show: true, jobIdentify, title, newMaxMessageId, data, settingName });
    };

    handleChange = index => {
        const checked = [...this.state.checked];
        checked[index] = !checked[index];
        this.setState({ checked: checked });

        var numberEqualCurrentNew = 0;
        const flagCheckTrue = checked;
        checked.filter((item, key) => {
            if (!this.isNotEqualCurrentNewByIndex(key)) {
                flagCheckTrue[key] = true;
                numberEqualCurrentNew++;
            }
        });

        if (flagCheckTrue.every(item => item === true)) {
            this.setState({ allChecked: true, isSubmitting: false });
        } else if (checked.every(item => item === false)) {
            this.setState({ allChecked: false, isSubmitting: true });
        } else if (numberEqualCurrentNew === checked.filter(item => item === true).length) {
            this.setState({ allChecked: false, isSubmitting: true });
        } else {
            this.setState({ allChecked: false, isSubmitting: false });
        }
    };

    isNotEqualCurrentNewByIndex = index => {
        const { currentMessages } = this.props;
        if (currentMessages.data[index]?.maxReferenceId !== currentMessages.data[index]?.referenceValue) {
            return true;
        }
        return false;
    };

    isAllEqualCurrentNew = () => {
        let isDisable = true;
        this.props.selected.map((item, key) => {
            if (this.isNotEqualCurrentNewByIndex(key)) {
                isDisable = false;
            }
        });

        return isDisable;
    };

    handleAllChange = () => {
        if (this.isAllEqualCurrentNew()) {
            return;
        }
        const { allChecked, checked } = this.state;
        const updatedChecked = checked.map(_ => !allChecked);
        this.setState({
            allChecked: !allChecked,
            checked: updatedChecked,
            isSubmitting: allChecked
        });
    };

    handleShowConfirm = () => {
        this.setState({ isShowConfirmPopup: true });
    };

    handleCloseConfirm = () => {
        this.setState({ isShowConfirmPopup: false });
    };

    isCheckedSubmit = () => {
        const { selected } = this.props;
        for (let index = 0; index < selected.length; index++) {
            if (this.isNotEqualCurrentNewByIndex(index) && this.state.checked[index] && selected[index].checked) {
                return true;
            }
        }
        return false;
    };

    setSubmit = async () => {
        await this.getAllData();
        this.isCheckedSubmit() && this.setState({ isSubmitting: false });
        if (this.isAllEqualCurrentNew()) {
            this.setState({ allChecked: false });
        }
    };

    isSelectableItem = (jobId, settingName) => {
        const currentMsg = this.props.currentMessages.data.filter(i => i.jobIdentify === jobId && i.settingName === settingName);
        return currentMsg !== null &&
            currentMsg.length > 0 &&
            currentMsg[0].referenceValue < currentMsg[0].maxReferenceId;
    };

    getSettingJobItem = (jobIdentifyId, settingName) => {
        const { selected } = this.props;
        return selected.find(item => item.id === jobIdentifyId && item.settingName === settingName);
    };

    handleSubmit = async () => {
        this.setState({ isSubmitting: true });
        const { selected } = this.props;
        const { checked } = this.state;
        const jobMsgs = [];
        selected.map((item, index) => {
            if (checked[index] && this.isSelectableItem(item.id, item.settingName)) {
                jobMsgs.push({
                    jobIdentify: item.id,
                    messageId: item.id !== 9 ? item.newMaxMessageId : dateFormat(item.newMaxMessageId, 'yyyy-mm-dd HH:MM:ss'),
                    notificationId: item.id !== 9 ? 0 : parseInt(item.notificationId, 10),
                    settingName: item.settingName
                });
            }
        });

        this.setState({ isShowConfirmPopup: false });

        await this.props.createMissingMessage(jobMsgs).then(response => {
            this.setState({ editResult: response.data });
            if (response.data.isSuccessful) {
                this.setSubmit();
            } else {
                this.setState({ isSubmitting: false });
            }
            setTimeout(() => {
                this.setState({ editResult: null });
            }, 3000);
        });
    };

    buildHeader = isHost => {
        return (
            <Row>
                {
                    isHost && <Cell colWidth={6} header>
                        <Input type="checkbox"
                            checked={this.state.allChecked}
                            onChange={() => this.handleAllChange()}
                            disabled={this.isAllEqualCurrentNew()}
                        />
                    </Cell>
                }
                <Cell colWidth={30} header>{Labels.Service}</Cell>
                <Cell colWidth={30} header>{Labels.CurrentPreviousMaxID}</Cell>
                <Cell colWidth={30} header>{Labels.NewPreviousMaxID}</Cell>
            </Row>
        );
    };

    buildBody = isHost => {
        const data = [];
        this.props.currentMessages.data.map((item, index) => {
            if (this.props.selected[index]?.checked) {
                data.push(item);
            }
        });
        if (data.length > 0) {
            return data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan={isHost ? 4 : 3}><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index, isHost) => (
        <Row key={`${item.jobIdentify}-${index}-${item.settingName}`} >
            <Cell className="text-center">
                {
                    <Input type="checkbox"
                        checked={this.state.checked[index] && this.isNotEqualCurrentNew(item.jobIdentify, item.settingName)}
                        onChange={event => this.handleChange(index)}
                        value={item.jobIdentify - 1}
                        disabled={!this.isNotEqualCurrentNew(item.jobIdentify, item.settingName)}
                    />
                }
            </Cell>
            <Cell>{this.getSettingJobItem(item.jobIdentify, item.settingName)?.name}</Cell>
            <Cell className="text-center">{item.referenceValue}</Cell>
            <Cell className="text-center">
                <div className="new-message">
                    <span className="messageId">{this.getSettingJobItem(item.jobIdentify, item.settingName)?.newMaxMessageId}</span>
                    {
                        (isHost && this.isNotEqualCurrentNew(item.jobIdentify, item.settingName)) &&
                        <span className="icon-edit messageIcon" onClick={() => this.handleShowEditNewMessage(item.jobIdentify, item.settingName)} />
                    }
                </div>
            </Cell>
        </Row>
    );

    render() {
        const { currentMessages, haveHostPermission } = this.props;
        const { isSubmitting, isShowConfirmPopup, editResult } = this.state;
        if (currentMessages === null)
            return null;

        return (
            <div className="table-container">
                {editResult &&
                    <ResponseMessage isSuccessful={editResult.isSuccessful}
                        message={editResult.message || Labels.ResetLastMessageIdSuccessfully} />
                }
                <Table
                    width={960}
                    autoWidth={false}
                    header={this.buildHeader(haveHostPermission)}
                    body={this.buildBody(haveHostPermission)}
                    containerPadding={0}
                    adjustedHeight={50}
                />
                <button className="btn-submit float-right mt-3" disabled={isSubmitting} onClick={() => this.handleShowConfirm()}>
                    {Labels.Proceed}
                </button>
                {
                    isShowConfirmPopup &&
                    <ConfirmPopup
                        onConfirm={this.handleSubmit}
                        onToggle={this.handleCloseConfirm}
                        isOpen={isShowConfirmPopup}
                        content={Labels.ConfirmResetLastId}
                        warningIcon={false}
                    />
                }
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { selected, currentMessages } = state;
    return { selected, currentMessages };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);