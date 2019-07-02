import React, { Component } from 'react';
import PropTypes from 'prop-types';

const wrapperClasses = {
    OVERLAY: 'overlay',
    OVERLAY_OPEN: 'overlay-open'
};

export default class Overlay extends Component {
    static propTypes = {
        closeOnOutsideClick: PropTypes.bool,
        isOpening: PropTypes.bool,
        onClose: PropTypes.func,
        onOpen: PropTypes.func
    }

    static defaultProps = {
        closeOnOutsideClick: true,
        isOpening: false
    }

    constructor(props) {
        super(props);

        this.containerElement = null;
        this.refHandlers = {
            container: ref => { this.containerElement = ref; }
        };

        this.state = {
            isOpening: this.props.isOpening
        };
    }

    componentDidMount() {
        this._registerEvents();
    }

    componentDidUpdate(prevProps) {
        if (this.props.isOpening !== prevProps.isOpening) {
            const { onOpen } = this.props;
            this.props.isOpening && onOpen && onOpen();
            this.setState({ isOpening: this.props.isOpening });
        }
    }

    componentWillUnmount() {
        this._unRegisterEvents();
    }

    _registerEvents() {
        const { closeOnOutsideClick } = this.props;

        if (closeOnOutsideClick) {
            document.addEventListener('click', this._handleClickOutSide, true);
        }
    }

    _unRegisterEvents = () => {
        const { closeOnOutsideClick } = this.props;

        if (closeOnOutsideClick) {
            document.removeEventListener('click', this._handleClickOutSide, true);
        }
    }

    _handleClickOutSide = event => {
        const clickOutside = this.containerElement && !this.containerElement.contains(event.target);

        if (clickOutside && this.state.isOpening) {
            this._close();
        }
    }

    _close() {
        const { onClose } = this.props;
        this.setState({ isOpening: false }, () => onClose && onClose());
    }

    render() {
        const { className, children } = this.props;
        const mergedClassName = wrapperClasses.OVERLAY + ' ' + className;
        const style = {
            display: this.state.isOpening ? 'block' : 'none'
        };

        return (
            <div className={this.state.isOpening ? mergedClassName + ' ' + wrapperClasses.OVERLAY_OPEN : mergedClassName}
                style={style} ref={this.refHandlers.container}>
                {children}
            </div>
        );
    }
}