import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Input } from 'reactstrap';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export class DataGrid extends Component {

    constructor(props) {
        super(props);

        this.state = {
            checked: [false, false, false],
            allChecked: false,
            isSubmitting: true
        };
    }

    componentDidMount() {
        this.getAllData();
    }

    getAllData = async () => {
        const messageIds = [];
        await this.props.getCurrentPreviousMaxMessage();

        const { currentMessages } = this.props;
        currentMessages.data.map(item => messageIds.push(item.messageId));
        await this.props.getNewPreviousMaxMessage({ messages: messageIds });

        const { newMessages } = this.props;
        const messageIdJobTwo = currentMessages.data.find(item => item.jobIdentify === 2).messageId;
        const paramMissingMessageCsd = {
            fromMessageId: messageIdJobTwo,
            toMessageId: newMessages.newMaxMessageId,
            postTime: newMessages.messagePostTimeInfos.find(item => item.messageId === messageIdJobTwo).postTime
        }
        await this.props.getMissingMessageCsd(paramMissingMessageCsd);

        const { missingMessageCsd } = this.props;
        const messages = currentMessages.data.map(item => {
            return {
                jobIdentify: item.jobIdentify,
                fromMessageId: item.messageId,
                toMessageId: newMessages.newMaxMessageId,
                postTime: newMessages.messagePostTimeInfos.find(element => element.messageId === item.messageId).postTime
            }
        });
        const paramMissingMessage = {
            messages: messages,
            msgContentTypes: currentMessages.messageTypeInfos,
            requests: missingMessageCsd
        };
        await this.props.getMissingMessageInfo(paramMissingMessage);

        const { missingMessages, selected } = this.props;
        const newMaxMessageIds = selected.map((item,index) => {
            const element = {...item};
            const newMaxMessageId = missingMessages.find(el => el.jobIdentify === item.id);
            if (newMaxMessageId) {
                element.newMaxMessageId = newMaxMessageId.messageId;
            } else {
                element.newMaxMessageId = currentMessages.data[index].messageId
            }
            return element;
        });
        this.props.setSelected(newMaxMessageIds);
    };

    isNotEqualCurrentNew = index => {
        const { selected, currentMessages } = this.props;
        if (selected[index].newMaxMessageId !== currentMessages.data[index].messageId) {
            return true;
        }
        return false;
    };

    handleShowEditNewMessage = jobIdentify => {
        const title = this.props.selected.find(item => item.id === jobIdentify).name;
        const newMaxMessageId = this.props.selected.find(item => item.id === jobIdentify).newMaxMessageId;
        this.props.setEditNewMessagePopup({ show: true, jobIdentify, title, newMaxMessageId });
    };

    handleChange = event => {
        const index = parseInt(event.target.value, 10);
        const checked = [...this.state.checked];
        checked[index] = !checked[index];
        this.setState({ checked: checked });

        this.props.selected.map((item, key) => {
            if (!this.isNotEqualCurrentNew(key) || !item.checked) {
                checked[key] = false;
            }
        });
        if (!checked.includes(true)) {
            this.setState({ isSubmitting: true });
        } else {
            this.setState({ isSubmitting: false });
        }
    };

    handleAllChange = () => {
        const allChecked = this.state.allChecked;
        if (!this.isNotEqualCurrentNew(0) && !this.isNotEqualCurrentNew(1) && !this.isNotEqualCurrentNew(2)) {
            return;
        };
        this.setState({ 
            allChecked: !allChecked,
            checked: [!allChecked,!allChecked,!allChecked],
            isSubmitting: allChecked
        });
    };

    checkCondition = (item, index) => {
        const { selected, currentMessages } = this.props;
        const { checked } = this.state;
        if (item.messageId > currentMessages.data[index].messageId && item.messageId <= selected[index].newMaxMessageId && checked[index]) {
            return true;
        }
        return false;
    };

    handleSubmit = async () => {
        this.setState({ isSubmitting: true });
        const { missingMessages, missingMessageCsd, selected } = this.props;
        const { checked } = this.state;
        const messages = [], requests = [], jobInfoList = [];
        missingMessages.filter(item => {
            if (item.jobIdentify === 1 && this.checkCondition(item, 0) && selected[0].checked) {
                messages.push({jobIdentify: 1, messageId: item.messageId});
            } else if (item.jobIdentify === 2 && this.checkCondition(item, 1) && selected[1].checked) {
                messages.push({jobIdentify: 2, messageId: item.messageId});
            } else if (item.jobIdentify === 3 && this.checkCondition(item, 2) && selected[2].checked){
                messages.push({jobIdentify: 3, messageId: item.messageId});
            }
        });
        missingMessageCsd.filter(item => {
            if (this.checkCondition(item, 1) && selected[1].checked) {
                requests.push(item);
            }
        });
        selected.map((item, index) => {
            if (item.checked && item.newMaxMessageId !== 0 && checked[index] && this.isNotEqualCurrentNew(index)) {
                jobInfoList.push({ jobIdentify: item.id, messageId: item.newMaxMessageId });
            }
        });

        await this.props.createMissingMessage({ messages: messages, requests: requests });
        await this.props.editMessageInfo({ jobInfoList: jobInfoList });
        await this.getAllData();
        this.setState({ isSubmitting: false });
    };

    buildHeader = isHost => {
        return (
            <Row>
                {
                    isHost && <Cell colWidth={6} header>
                        <Input type="checkbox"
                            defaultChecked={this.state.allChecked}
                            onChange={() => this.handleAllChange()} />
                    </Cell>
                }
                <Cell colWidth={30} header>{Labels.System}</Cell>
                <Cell colWidth={30} header>{Labels.CurrentPreviousMaxMessageID}</Cell>
                <Cell colWidth={30} header>{Labels.NewPreviousMaxMessageID}</Cell>
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
                    this.isNotEqualCurrentNew(item.jobIdentify-1) &&
                    <Input type="checkbox"
                        checked={this.state.checked[item.jobIdentify-1]}
                        onChange={event => this.handleChange(event)}
                        value={item.jobIdentify-1}
                    />
                }
            </Cell>
            <Cell>{this.props.selected[item.jobIdentify-1].name}</Cell>
            <Cell className="text-center">{item.messageId}</Cell>
            <Cell className="text-center">
                <div className="new-message">
                    <span className="messageId">{this.props.selected[item.jobIdentify-1].newMaxMessageId}</span>
                    {
                       (isHost && this.isNotEqualCurrentNew(item.jobIdentify-1)) &&
                       <span className="icon-edit messageIcon" onClick={() => this.handleShowEditNewMessage(item.jobIdentify)}/>
                    }
                </div>
            </Cell>
        </Row>
    );

    render() {
        const { currentMessages, newMessages, missingMessages, haveHostPermission } = this.props;
        const { isSubmitting } = this.state;
        if (currentMessages === null || newMessages === null || missingMessages === null)
            return null;

        return (
            <div className="table-container">
                <Table
                    width={960}
                    autoWidth={false}
                    header={this.buildHeader(haveHostPermission)}
                    body={this.buildBody(haveHostPermission)}
                    containerPadding={0}
                    adjustedHeight={50}
                />
                <button className="btn-submit float-right mt-3" disabled={isSubmitting} onClick={() => this.handleSubmit()}>
                    {Labels.Proceed}
                </button>
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { selected, currentMessages, newMessages, missingMessageCsd, missingMessages } = state;
    return { selected, currentMessages, newMessages, missingMessageCsd, missingMessages };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
