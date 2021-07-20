jest.mock('react-redux');
jest.mock('redux');
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';

describe('connect', () => {
    let mapStateToProps, mapDispatchToProps;
    const connectMock = jest.fn();

    beforeAll(() => {
        connect.mockImplementation((mapStateToPropsFunc, mapDispatchToPropsFunc) => {
            mapStateToProps = mapStateToPropsFunc;
            mapDispatchToProps = mapDispatchToPropsFunc;
            return connectMock;
        });
        require('../DataGridRecipientList').default;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('props should be mapped correctly', () => {
        // Arrange
        const expected = {
            paramsPageRecipientList: {
                pageIndex: 1,
                pageSize: 100
            },
            recipientList: {}
        };
        const state = {
            paramsPageRecipientList: {
                pageIndex: 1,
                pageSize: 100
            },
            recipientList: {}
        };

        // Act
        const actual = mapStateToProps(state);

        // Assert
        expect(actual).toEqual(expected);
    });

    it('bindActionCreators should be called with correct parameter', () => {
        // Arrange
        const mockBindAction = jest.fn();
        bindActionCreators.mockImplementationOnce(mockBindAction);
        const dispatch = {};

        // Act
        mapDispatchToProps(dispatch);

        // Assert
        expect(mockBindAction).toHaveBeenCalledWith(ActionCreators, dispatch);
    });
})