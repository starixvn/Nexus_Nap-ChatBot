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

export class DataGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            checked: [false, false, false, false, false],
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

        const { currentMessages } = this.props;

        const { selected } = this.props;

        const newMaxMessageIds = selected.map((item, index) => {
            const element = { ...item };

            const newMaxMessageId = currentMessages.data.find(el => el.jobIdentify === item.id);
            element.newMaxMessageId = newMaxMessageId.maxReferenceId;

            return element;
        });

        this.props.setSelected(newMaxMessageIds);
    };

    isNotEqualCurrentNew = index => {
        const { selected, currentMessages } = this.props;
        if (selected[index].newMaxMessageId !== currentMessages.data[index].referenceId) {
            return true;
        }
        return false;
    };

    handleShowEditNewMessage = jobIdentify => {
        const title = this.props.selected.find(item => item.id === jobIdentify).name;
        const newMaxMessageId = this.props.selected.find(item => item.id === jobIdentify).newMaxMessageId;
        let data;
        data = this.props.currentMessages.missingMsgInfo.filter(x => x.jobIdentify === jobIdentify);
        this.props.setEditNewMessagePopup({ show: true, jobIdentify, title, newMaxMessageId, data });
    };

    handleChange = event => {
        const index = parseInt(event.target.value, 10);
        const checked = [...this.state.checked];
        checked[index] = !checked[index];
        this.setState({ checked: checked });

        const flagCheckTrue = checked;
        checked.filter((item, key) => {
            if (!this.isNotEqualCurrentNew(key)) {
                flagCheckTrue[key] = true;
            }
        });

        if (flagCheckTrue.every(item => item === true)) {
            this.setState({ allChecked: true, isSubmitting: false });
        } else if (checked.every(item => item === false)) {
            this.setState({ allChecked: false, isSubmitting: true });
        } else {
            this.setState({ allChecked: false, isSubmitting: false });
        }
    };

    isAllEqualCurrentNew = () => {
        let isDisable = true;
        this.props.selected.map((item, key) => {
            if (this.isNotEqualCurrentNew(key)) {
                isDisable = false;
            }
        });

        return isDisable;
    };

    handleAllChange = () => {
        const allChecked = this.state.allChecked;
        if (this.isAllEqualCurrentNew()) {
            return;
        }
        this.setState({
            allChecked: !allChecked,
            checked: [!allChecked, !allChecked, !allChecked, !allChecked, !allChecked],
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
            if (this.isNotEqualCurrentNew(index) && this.state.checked[index] && selected[index].checked) {
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

    isSelectableItem = jobId => {
        const currentMsg = this.props.currentMessages.data.filter(i => i.jobIdentify === jobId);
        return currentMsg !== null &&
            currentMsg.length > 0 &&
            currentMsg[0].referenceId < currentMsg[0].maxReferenceId;
    }

    handleSubmit = async () => {
        this.setState({ isSubmitting: true });
        const { selected } = this.props;
        const { checked } = this.state;
        const jobMsgs = [];
        selected.map(item => {
            if (checked[item.id - 1] && this.isSelectableItem(item.id)) {
                jobMsgs.push({
                    jobIdentify: item.id,
                    messageId: item.newMaxMessageId
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
            if (this.props.selected[index].checked) {
                data.push(item);
            }
        });
        if (data.length > 0) {
            return data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan={isHost ? 4 : 3}><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index, isHost) => (
        <Row key={`${item.jobIdentify}-${index}`} >
            <Cell className="text-center">
                {
                    <Input type="checkbox"
                        checked={this.state.checked[item.jobIdentify - 1] && this.isNotEqualCurrentNew(item.jobIdentify - 1)}
                        onChange={event => this.handleChange(event)}
                        value={item.jobIdentify - 1}
                        disabled={!this.isNotEqualCurrentNew(item.jobIdentify - 1)}
                    />
                }
            </Cell>
            <Cell>{this.props.selected[item.jobIdentify - 1].name}</Cell>
            <Cell className="text-center">{item.referenceId}</Cell>
            <Cell className="text-center">
                <div className="new-message">
                    <span className="messageId">{this.props.selected[item.jobIdentify - 1].newMaxMessageId}</span>
                    {
                        (isHost && this.isNotEqualCurrentNew(item.jobIdentify - 1)) &&
                        <span className="icon-edit messageIcon" onClick={() => this.handleShowEditNewMessage(item.jobIdentify)} />
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